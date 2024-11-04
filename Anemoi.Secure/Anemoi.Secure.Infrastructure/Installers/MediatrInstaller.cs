using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.Secure.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Secure.Infrastructure.Installers;

public sealed class MediatrInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(ISecureApplicationAssemblyMarker).Assembly));
    }
}