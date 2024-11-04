using System.Reflection;
using System.Reflection.Emit;
using Anemoi.BuildingBlock.Application;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.EventDriven;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.Centralize.Application;
using Anemoi.Centralize.Application.Abstractions;
using Anemoi.Centralize.Application.ContractAssemblies;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneOf;

namespace Anemoi.Centralize.Infrastructure.Installers;

public class DefaultHandlersInstaller : IInstaller
{
    private static readonly Dictionary<Type, Type> RequestMapHandlers = new()
    {
        { typeof(IQueryCounting), typeof(IQueryCountingHandler<>) },
        { typeof(IQueryOne<>), typeof(IQueryOneHandler<,>) },
        { typeof(IQueryPaged<>), typeof(IQueryPagedHandler<,>) },
        { typeof(IQueryCollection<>), typeof(IQueryCollectionHandler<,>) },
        { typeof(ICommandVoid), typeof(ICommandVoidHandler<>) },
        { typeof(ICommandResult<>), typeof(ICommandResultHandler<,>) },
    };

    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        var ignoreTypesInNamespace = typeof(ICentralizeApplicationAssemblyMarker);

        var ignoreClasses = ignoreTypesInNamespace.Assembly.ExportedTypes
            .Where(x => typeof(IMessageHandler).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
            .Distinct()
            .ToList();

        var ignoreInterfaces = ignoreClasses.SelectMany(a => a.GetInterfaces()).ToList();

        var typeNamespace = typeof(IBuildingBlockApplicationAssemblyMarker).Namespace;
        var requestTypes = ContractAssembly.GetAllContractAssemblies().SelectMany(x => x.ExportedTypes)
            .Where(x => typeof(IMessage).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
            .ToList();

        var requestMapResponseTypes = new List<RequestMapResponseType>();

        foreach (var requestType in requestTypes)
        {
            var implementationTypes = requestType.GetInterfaces();
            var requestInterfaceType = implementationTypes.FirstOrDefault(i => !implementationTypes.Any(j =>
                j.GetInterfaces().Contains(i)) && i.Namespace?.Contains(typeNamespace!) == true);

            // Start checking interface type!
            if (requestInterfaceType == typeof(ICommandVoid))
            {
                var commandVoidType = RequestMapHandlers[requestInterfaceType].MakeGenericType(requestType);
                if (ignoreInterfaces.Contains(commandVoidType)) continue;
                requestMapResponseTypes.Add(new RequestMapResponseType(commandVoidType,
                    typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(OneOf<None, ErrorDetailResponse>))));
                continue;
            }

            if (requestInterfaceType == typeof(IQueryCounting))
            {
                var queryCountingType = RequestMapHandlers[requestInterfaceType].MakeGenericType(requestType);
                if (ignoreInterfaces.Contains(queryCountingType)) continue;
                requestMapResponseTypes.Add(new RequestMapResponseType(queryCountingType,
                    typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(CountingResponse))));
                continue;
            }
            
            if(requestInterfaceType is null) continue;

            var genericType = requestInterfaceType.GetGenericTypeDefinition();
            
            if (genericType == typeof(IQueryOne<>))
            {
                var requestMatchingType = RequestMapHandlers.FirstOrDefault(x =>
                    implementationTypes.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == x.Key));
                var responseType = requestInterfaceType.GenericTypeArguments.FirstOrDefault();
                if (responseType is null || requestMatchingType.Value is null) continue;
                var newType = requestMatchingType.Value.MakeGenericType(requestType, responseType);
                if (ignoreInterfaces.Contains(newType)) continue;
                requestMapResponseTypes.Add(new RequestMapResponseType(newType,
                    typeof(IRequestHandler<,>).MakeGenericType(requestType,
                        typeof(OneOf<,>).MakeGenericType(responseType, typeof(ErrorDetailResponse)))));
                continue;
            }

            if (genericType == typeof(IQueryPaged<>))
            {
                var requestMatchingType = RequestMapHandlers.FirstOrDefault(x =>
                    implementationTypes.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == x.Key));
                var responseType = requestInterfaceType.GenericTypeArguments.FirstOrDefault();
                if (responseType is null || requestMatchingType.Value is null) continue;
                var newType = requestMatchingType.Value.MakeGenericType(requestType, responseType);
                if (ignoreInterfaces.Contains(newType)) continue;
                requestMapResponseTypes.Add(new RequestMapResponseType(newType,
                    typeof(IRequestHandler<,>).MakeGenericType(requestType,
                        typeof(PaginationResponse<>).MakeGenericType(responseType))));
                continue;
            }

            if (genericType == typeof(IQueryCollection<>))
            {
                var requestMatchingType = RequestMapHandlers.FirstOrDefault(x =>
                    implementationTypes.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == x.Key));
                var responseType = requestInterfaceType.GenericTypeArguments.FirstOrDefault();
                if (responseType is null || requestMatchingType.Value is null) continue;
                var newType = requestMatchingType.Value.MakeGenericType(requestType, responseType);
                if (ignoreInterfaces.Contains(newType)) continue;
                requestMapResponseTypes.Add(new RequestMapResponseType(newType,
                    typeof(IRequestHandler<,>).MakeGenericType(requestType,
                        typeof(CollectionResponse<>).MakeGenericType(responseType))));
                continue;
            }

            if (genericType == typeof(ICommandResult<>))
            {
                var requestMatchingType = RequestMapHandlers.FirstOrDefault(x =>
                    implementationTypes.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == x.Key));
                var responseType = requestInterfaceType.GenericTypeArguments.FirstOrDefault();
                if (responseType is null || requestMatchingType.Value is null) continue;
                var newType = requestMatchingType.Value.MakeGenericType(requestType, responseType);
                if (ignoreInterfaces.Contains(newType)) continue;
                requestMapResponseTypes.Add(new RequestMapResponseType(newType,
                    typeof(IRequestHandler<,>).MakeGenericType(requestType,
                        typeof(OneOf<,>).MakeGenericType(responseType, typeof(ErrorDetailResponse)))));
            }
        }

        if (requestMapResponseTypes is not { Count: > 0 }) return;

        var assemblyName = new AssemblyName { Name = "DynamicInstanceAssemblyHandlers" };
        var newAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var newModule = newAssembly.DefineDynamicModule("DynamicInstanceModule");
        var typeBuilder = newModule.DefineType("DefaultHandlers", TypeAttributes.Public, null,
            requestMapResponseTypes.Select(a => a.InterfaceType).ToArray());

        // Add the constructor
        var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
            [typeof(IRequestClientService)]);

        var ctorGenerator = ctorBuilder.GetILGenerator();
        ctorGenerator.Emit(OpCodes.Ldarg_0); // Load "this" onto the stack
        ctorGenerator.Emit(OpCodes.Ldarg_1); // Load "IRequestClientRepository" onto the stack
        var repository = typeBuilder.DefineField("requestClientService", typeof(IRequestClientService),
            FieldAttributes.Private);
        ctorGenerator.Emit(OpCodes.Stfld,
            repository); // Assign "requestClientRepository" to the private field "IRequestClientRepository"
        ctorGenerator.Emit(OpCodes.Ret); // Return from the constructor

        // Add the IRequestClientRepository property
        var requestClientRepository = typeBuilder.DefineProperty("RequestClientService", PropertyAttributes.None,
            typeof(IRequestClientService), null);
        var requestClientRepositoryGetMethod = typeBuilder.DefineMethod("get_RequestClientService",
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig |
            MethodAttributes.Virtual, typeof(IRequestClientService), Type.EmptyTypes);
        var methodGenerator = requestClientRepositoryGetMethod.GetILGenerator();
        methodGenerator.Emit(OpCodes.Ldarg_0); // Load "this" onto the stack
        methodGenerator.Emit(OpCodes.Ldfld,
            repository); // Load the value of the field "IRequestClientRepository" onto the stack`
        methodGenerator.Emit(OpCodes.Ret); // Return from the getter method
        requestClientRepository.SetGetMethod(requestClientRepositoryGetMethod);
        var handlersType = typeBuilder.CreateType();
        requestMapResponseTypes.ForEach(c => services.AddTransient(c.HandlerType, handlersType));
    }

    private sealed record RequestMapResponseType(Type InterfaceType, Type HandlerType);
}