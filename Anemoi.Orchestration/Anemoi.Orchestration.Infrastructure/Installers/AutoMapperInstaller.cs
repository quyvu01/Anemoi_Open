using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.Orchestrator.Machine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Orchestrator.Infrastructure.Installers;

public sealed class AutoMapperInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(IOrchestrationMachineAssemblyMarker));
    }
}