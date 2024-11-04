using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Configurations;
using Anemoi.BuildingBlock.Infrastructure.Filters;
using Anemoi.BuildingBlock.Infrastructure.HandlerConsumers;
using Anemoi.Contract.Workspace;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Anemoi.Workspace.Application;

namespace Anemoi.Workspace.Infrastructure.Installers;

public sealed class MassTransitInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        var masstransitSetting = configuration.GetSection(nameof(MassTransitSetting)).Get<MassTransitSetting>()!;
        var (host, virtualHost, userName, password, _, _) = masstransitSetting;
        services.AddMassTransit(configurator =>
        {
            configurator.SetKebabCaseEndpointNameFormatter();
            configurator.AddConsumersFromNamespaceContaining<IWorkspaceApplicationAssemblyMarker>();
            var serviceConsumer = ConsumersHelper
                .CreateDynamicConsumerHandlers<IWorkspaceContractAssemblyMarker>("WorkspaceHandlersConsumer");
            configurator.AddConsumer(serviceConsumer);
            configurator.UsingRabbitMq((context, bus) =>
            {
                bus.Host(host, virtualHost, c =>
                {
                    c.Username(userName);
                    c.Password(password);
                });
                bus.UseConsumeFilter(typeof(RequestHeaderFilter<>), context);
                bus.ConfigureEndpoints(context);
            });
        });
    }
}