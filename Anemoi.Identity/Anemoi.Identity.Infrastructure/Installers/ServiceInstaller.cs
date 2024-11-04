using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Infrastructure.GeneralInstaller;
using Anemoi.BuildingBlock.Infrastructure.Services;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Domain;
using Anemoi.Identity.Domain.Models;
using Anemoi.Identity.Infrastructure.DataContext;
using Anemoi.Identity.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anemoi.Identity.Infrastructure.Installers;

public sealed class ServiceInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddHttpClient();

        services.TryAddScoped<ISqlRepository<Role>, UserRoleRepository>();
        services.TryAddScoped<ISignInRepository, SignInRepository>();
        services.TryAddScoped<IUserRepository, UserRepository>();
        services.TryAddScoped<ISqlRepository<User>, UserRepository>();
        services.TryAddScoped<IUserClaimRepository, UserClaimRepository>();
        services.AddEfRepositoriesAsScope<IdentityDbContext>(typeof(IIdentityDomainAssemblyMarker).Assembly);
        services.AddEfUnitOfWorkAsScope<IdentityDbContext>();
        services.AddScoped<IUserIdSetter, UserService>();
        services.AddScoped<IUserIdGetter>(sp => sp.GetRequiredService<IUserIdSetter>() as UserService);
        services.AddScoped<IWorkspaceIdSetter, WorkspaceService>();
        services.AddScoped<IWorkspaceIdGetter>(sp => sp.GetRequiredService<IWorkspaceIdSetter>() as WorkspaceService);
        services.AddScoped<IAdministratorSetter, AdministratorService>();
        services.AddScoped<IAdministratorGetter>(sp => sp.GetRequiredService<IAdministratorSetter>() as AdministratorService);
        services.AddScoped<IApplicationPolicySetter, ApplicationPolicyService>();
        services.AddScoped<IApplicationPolicyGetter>(sp => sp.GetRequiredService<IApplicationPolicySetter>() as ApplicationPolicyService);        
        services.AddScoped<ITokenSetter, TokenService>();
        services.AddScoped<ITokenGetter>(sp => sp.GetRequiredService<ITokenSetter>() as TokenService);
    }
}