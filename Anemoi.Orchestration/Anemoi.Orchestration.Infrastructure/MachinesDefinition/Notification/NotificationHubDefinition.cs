using Anemoi.Orchestration.Contract.NotificationContract.Instances;
using MassTransit;

namespace Anemoi.Orchestrator.Infrastructure.MachinesDefinition.Notification;

public class NotificationHubDefinition : SagaDefinition<NotificationHubInstance>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<NotificationHubInstance> sagaConfigurator,
        IRegistrationContext context)
    {
        sagaConfigurator.UseDelayedRedelivery(r =>
            r.Intervals(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(5)));
        sagaConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 1000, 1000, 1000, 1000, 1000));
        sagaConfigurator.UseInMemoryOutbox(context);
        base.ConfigureSaga(endpointConfigurator, sagaConfigurator, context);
    }
}