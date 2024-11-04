using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Configurations;
using Anemoi.BuildingBlock.Infrastructure.Filters;
using Anemoi.BuildingBlock.Infrastructure.HandlerConsumers;
using Anemoi.Contract.Identity;
using MassTransit;
using Anemoi.Identity.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Identity.Infrastructure.Installers;

public sealed class MassTransitInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        var masstransitSetting = configuration.GetSection(nameof(MassTransitSetting)).Get<MassTransitSetting>()!;
        var (host, virtualHost, userName, password, _, _) = masstransitSetting;
        services.AddMassTransit(configurator =>
        {
            configurator.SetKebabCaseEndpointNameFormatter();
            configurator.AddConsumersFromNamespaceContaining<IIdentityApplicationAssemblyMarker>();
            var serviceConsumer = ConsumersHelper
                .CreateDynamicConsumerHandlers<IIdentityContractAssemblyMarker>("IdentityHandlersConsumer");
            configurator.AddConsumer(serviceConsumer);
            configurator.UsingRabbitMq((context, bus) =>
            {
                bus.Host(host, virtualHost, c =>
                {
                    c.Username(userName);
                    c.Password(password);
                });
                bus.ConfigureEndpoints(context);
                bus.UseConsumeFilter(typeof(RequestHeaderFilter<>), context);
            });
        });
    }
}