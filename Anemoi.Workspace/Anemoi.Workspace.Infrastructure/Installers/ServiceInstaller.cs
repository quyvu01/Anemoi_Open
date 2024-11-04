using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Infrastructure.GeneralInstaller;
using Anemoi.BuildingBlock.Infrastructure.Services;
using Anemoi.Workspace.Domain;
using Anemoi.Workspace.Infrastructure.DataContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Workspace.Infrastructure.Installers;

public sealed class ServiceInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddEfRepositoriesAsScope<WorkspaceDbContext>(typeof(IWorkspaceDomainAssemblyMarker).Assembly);
        services.AddEfUnitOfWorkAsScope<WorkspaceDbContext>();
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