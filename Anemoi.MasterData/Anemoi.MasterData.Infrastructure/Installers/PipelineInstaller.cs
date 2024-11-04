using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Pipelines;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.MasterData.Infrastructure.Installers;

public sealed class PipelineInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TimeMeasuringBehavior<,>));
    }
}