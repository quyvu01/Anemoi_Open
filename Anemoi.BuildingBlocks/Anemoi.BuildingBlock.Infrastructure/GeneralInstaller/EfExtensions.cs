using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Domain;
using Anemoi.BuildingBlock.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

namespace Anemoi.BuildingBlock.Infrastructure.GeneralInstaller;

public static class EfExtensions
{
    public static void AddEfRepositoriesAsScope<TDbContext>(this IServiceCollection services, Assembly modelAssembly)
        where TDbContext : DbContext => modelAssembly.ExportedTypes
        .Where(x => typeof(ValueObject).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
        .ForEach(x =>
        {
            var assemblyName = new AssemblyName
                { Name = $"{typeof(TDbContext).Assembly.GetName().Name}.DynamicEfRepositoryType" };
            var newAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var newModule = newAssembly.DefineDynamicModule("DynamicEfRepositoryModule");
            var efRepositoryType = typeof(EfRepository<>).MakeGenericType(x);
            var typeBuilder = newModule.DefineType($"{x.Name}Repository", TypeAttributes.Public, efRepositoryType);
            var ctorTypes = efRepositoryType.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance, [typeof(TDbContext), typeof(ILogger)])!;
            // Define the constructor for the dynamic class
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard, [typeof(TDbContext), typeof(ILogger)]);
            // Generate the constructor IL code
            var ilGenerator = constructorBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0); // Load "this" onto the stack
            ilGenerator.Emit(OpCodes.Ldarg_1); // Load the TDbContext argument onto the stack
            ilGenerator.Emit(OpCodes.Ldarg_2); // Load the ILogger argument onto the stack
            // Call the base constructor with the TDbContext and ILogger arguments
            ilGenerator.Emit(OpCodes.Call, ctorTypes);
            ilGenerator.Emit(OpCodes.Ret); // Return from the constructor
            var repositoryType = typeBuilder.CreateType();
            var implementationType = typeof(ISqlRepository<>).MakeGenericType(x);
            services.TryAddScoped(implementationType, repositoryType);
        });

    public static void AddEfUnitOfWorkAsScope<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        var assemblyName = new AssemblyName
            { Name = $"{typeof(TDbContext).Assembly.GetName().Name}.DynamicEfUnitOfWorkType" };
        var newAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var newModule = newAssembly.DefineDynamicModule("DynamicEfUnitOfWorkModule");
        var efUnitOfWorkType = typeof(EfUnitOfWork);
        var typeBuilder = newModule.DefineType("UnitOfWork", TypeAttributes.Public, efUnitOfWorkType);
        var ctorTypes = efUnitOfWorkType
            .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, [typeof(TDbContext), typeof(ILogger)])!;
        // Define the constructor for the dynamic class
        var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public,
            CallingConventions.Standard, [typeof(TDbContext), typeof(ILogger)]);
        // Generate the constructor IL code
        var ilGenerator = constructorBuilder.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0); // Load "this" onto the stack
        ilGenerator.Emit(OpCodes.Ldarg_1); // Load the TDbContext argument onto the stack
        ilGenerator.Emit(OpCodes.Ldarg_2); // Load the ILogger argument onto the stack
        // Call the base constructor with the TDbContext and ILogger arguments
        ilGenerator.Emit(OpCodes.Call, ctorTypes);
        ilGenerator.Emit(OpCodes.Ret); // Return from the constructor
        var unitOfWorkType = typeBuilder.CreateType();
        services.TryAddScoped(typeof(IUnitOfWork), unitOfWorkType);
    }
}