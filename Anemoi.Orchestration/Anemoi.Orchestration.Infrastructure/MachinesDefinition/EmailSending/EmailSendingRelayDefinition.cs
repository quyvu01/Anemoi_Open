using Anemoi.Orchestration.Contract.EmailSendingContract.Instances;
using MassTransit;

namespace Anemoi.Orchestrator.Infrastructure.MachinesDefinition.EmailSending;

public sealed class EmailSendingRelayDefinition : SagaDefinition<EmailSendingRelayInstance>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<EmailSendingRelayInstance> sagaConfigurator,
        IRegistrationContext context)
    {
        sagaConfigurator.UseDelayedRedelivery(r =>
            r.Intervals(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(5)));
        sagaConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 1000, 1000, 1000, 1000, 1000));
        sagaConfigurator.UseInMemoryOutbox(context);
        base.ConfigureSaga(endpointConfigurator, sagaConfigurator, context);
    }
}