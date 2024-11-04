using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using MassTransit;
using MediatR;
using Anemoi.BuildingBlock.Application;
using Anemoi.BuildingBlock.Application.Consumers;
using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.EventDriven;

namespace Anemoi.BuildingBlock.Infrastructure.HandlerConsumers;

public static class ConsumersHelper
{
    private static readonly Dictionary<Type, Type> RequestMapHandlers = new()
    {
        { typeof(IQueryCounting), typeof(IQueryCountingConsumer<>) },
        { typeof(IQueryOne<>), typeof(IQueryOneConsumer<,>) },
        { typeof(IQueryPaged<>), typeof(IQueryPagedConsumer<,>) },
        { typeof(IQueryCollection<>), typeof(IQueryCollectionConsumer<,>) },
        { typeof(ICommandVoid), typeof(ICommandVoidConsumer<>) },
        { typeof(ICommandResult<>), typeof(ICommandResultConsumer<,>) },
    };

    public static Type CreateDynamicConsumerHandlers<TContractAssemblyMarker>(string consumerName)
    {
        var typeNamespace = typeof(IBuildingBlockApplicationAssemblyMarker).Namespace;
        var requestTypes = typeof(TContractAssemblyMarker).Assembly.ExportedTypes.Where(x =>
            typeof(IMessage).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false }).ToList();
        var allConsumerType = new List<Type>();
        foreach (var requestType in requestTypes)
        {
            var implementationTypes = requestType.GetInterfaces();
            var requestInterfaceType = implementationTypes.FirstOrDefault(i => !implementationTypes.Any(j =>
                j.GetInterfaces().Contains(i)) && i.Namespace?.Contains(typeNamespace!) == true);
            if (requestInterfaceType == typeof(ICommandVoid) || requestInterfaceType == typeof(IQueryCounting))
            {
                var newCommandVoidConsumerType = RequestMapHandlers[requestInterfaceType].MakeGenericType(requestType);
                allConsumerType.Add(newCommandVoidConsumerType);
                continue;
            }

            var requestMatchingType = RequestMapHandlers.FirstOrDefault(x =>
                implementationTypes.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == x.Key));
            var responseType = requestInterfaceType?.GenericTypeArguments.FirstOrDefault();
            if (responseType is null || requestMatchingType.Value is null) continue;
            var newType = requestMatchingType.Value.MakeGenericType(requestType, responseType);
            allConsumerType.Add(newType);
        }

        if (allConsumerType is not { Count: > 0 })
            throw new ConsumerException("The dynamic consumer should be implemented at least one IConsumer<T>!");

        var assemblyName = new AssemblyName { Name = "DynamicInterfaceAssemblyHandlers" };
        var newAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var newModule = newAssembly.DefineDynamicModule("DynamicInterfaceModule");
        var typeBuilder = newModule.DefineType(consumerName, TypeAttributes.Public, null, allConsumerType.ToArray());

        // Add the constructor
        var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
            [typeof(IMediator)]);

        var ctorGenerator = ctorBuilder.GetILGenerator();
        ctorGenerator.Emit(OpCodes.Ldarg_0); // Load "this" onto the stack
        ctorGenerator.Emit(OpCodes.Ldarg_1); // Load "IMediator" onto the stack
        var mediatorField = typeBuilder.DefineField("mediator", typeof(IMediator), FieldAttributes.Private);
        ctorGenerator.Emit(OpCodes.Stfld, mediatorField); // Assign "mediator" to the private field "IMediator"
        ctorGenerator.Emit(OpCodes.Ret); // Return from the constructor

        // Add the IMediator property
        var mediatorProperty = typeBuilder.DefineProperty("Mediator", PropertyAttributes.None, typeof(IMediator), null);
        var mediatorGetMethod = typeBuilder.DefineMethod("get_Mediator",
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig |
            MethodAttributes.Virtual, typeof(IMediator), Type.EmptyTypes);
        var methodGenerator = mediatorGetMethod.GetILGenerator();
        methodGenerator.Emit(OpCodes.Ldarg_0); // Load "this" onto the stack
        methodGenerator.Emit(OpCodes.Ldfld, mediatorField); // Load the value of the field "IMediator" onto the stack`
        methodGenerator.Emit(OpCodes.Ret); // Return from the getter method
        mediatorProperty.SetGetMethod(mediatorGetMethod);
        return typeBuilder.CreateType();
    }
}