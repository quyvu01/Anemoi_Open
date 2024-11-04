using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.Identity.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Identity.Infrastructure.Installers;

public sealed class AutoMapperInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(IIdentityApplicationAssemblyMarker));
    }
}