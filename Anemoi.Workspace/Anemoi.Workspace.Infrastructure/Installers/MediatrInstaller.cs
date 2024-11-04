using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.Workspace.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Workspace.Infrastructure.Installers;

public sealed class MediatrInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(IWorkspaceApplicationAssemblyMarker).Assembly));
    }
}