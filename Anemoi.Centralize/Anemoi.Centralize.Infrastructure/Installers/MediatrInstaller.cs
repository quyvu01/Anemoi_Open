using Anemoi.BuildingBlock.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Anemoi.Centralize.Application;

namespace Anemoi.Centralize.Infrastructure.Installers;

public sealed class MediatrInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(ICentralizeApplicationAssemblyMarker).Assembly));
    }
}