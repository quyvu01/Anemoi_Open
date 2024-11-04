using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Configurations;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Anemoi.Centralize.Application;

namespace Anemoi.Centralize.Infrastructure.Installers;

public sealed class MassTransitInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        var masstransitSetting = configuration.GetSection(nameof(MassTransitSetting)).Get<MassTransitSetting>()!;
        var (host, virtualHost, userName, password, _, _) = masstransitSetting;
        services.AddMassTransit(configurator =>
        {
            configurator.SetKebabCaseEndpointNameFormatter();
            configurator.AddConsumersFromNamespaceContaining<ICentralizeApplicationAssemblyMarker>();
            configurator.AddActivitiesFromNamespaceContaining<ICentralizeApplicationAssemblyMarker>();
            configurator.UsingRabbitMq((context, bus) =>
            {
                bus.Host(host, virtualHost, c =>
                {
                    c.Username(userName);
                    c.Password(password);
                });
                bus.ConfigureEndpoints(context);
            });
        });
    }
}