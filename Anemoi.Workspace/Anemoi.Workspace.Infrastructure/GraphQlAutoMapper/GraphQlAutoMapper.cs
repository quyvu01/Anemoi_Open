using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.Contract.Workspace.Responses;
using Anemoi.Workspace.Application.Attributes;
using Anemoi.Workspace.Application.GraphQl.Queries;
using HotChocolate.Types;

namespace Anemoi.Workspace.Infrastructure.GraphQlAutoMapper;

public class GraphQlAutoMapper
{
    public static Type CreateGraphQlAutoMapper()
    {
        var assemblyName = new AssemblyName("DynamicAssembly");
        var assemblyBuilder =
            AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        var typeBuilder = moduleBuilder.DefineType("DynamicWorkspaceResponseType", TypeAttributes.Public,
            typeof(ObjectType<WorkspaceResponse>));

        var configureMethodBuilder = typeBuilder.DefineMethod("Configure",
            MethodAttributes.Public | MethodAttributes.Virtual,
            typeof(void),
            [typeof(IObjectTypeDescriptor<WorkspaceResponse>)]);

        var ilGenerator = configureMethodBuilder.GetILGenerator();

        var descriptorType = typeof(IObjectTypeDescriptor<WorkspaceResponse>);
        var resolverType = typeof(Resolvers);

        var resolverMethods = resolverType.GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(ResolverAttribute), false).Length > 0);

        foreach (var method in resolverMethods)
        {
            var attribute = (ResolverAttribute)method.GetCustomAttributes(typeof(ResolverAttribute), false).First();
            var fieldName = attribute.FieldName;

            var descriptorFieldMethod = descriptorType.GetMethod("Field", [typeof(string)]);

            ilGenerator.Emit(OpCodes.Ldarg_1); // Load the descriptor
            ilGenerator.Emit(OpCodes.Ldstr, fieldName); // Load the field name
            ilGenerator.Emit(OpCodes.Callvirt, descriptorFieldMethod!); // Call Field(fieldName)
            
            var resolveWithMethod = descriptorFieldMethod.ReturnType
                .GetMethods().FirstOrDefault(a => a.Name == "ResolveWith" && a.GetParameters().Length == 1);

            ilGenerator.Emit(OpCodes.Ldtoken, resolverType); // Load the resolver type
            ilGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle")!); // Get the Type object
            ilGenerator.Emit(OpCodes.Ldstr, method.Name); // Load the method name
            ilGenerator.Emit(OpCodes.Callvirt, resolveWithMethod!); // Call ResolveWith(type, methodName)
        }

        ilGenerator.Emit(OpCodes.Ret); // Return

        return typeBuilder.CreateType();
    }
}