using Anemoi.BuildingBlock.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Anemoi.Centralize.Application;

namespace Anemoi.Centralize.Infrastructure.Installers;

public sealed class AutoMapperInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(ICentralizeApplicationAssemblyMarker));
    }
}