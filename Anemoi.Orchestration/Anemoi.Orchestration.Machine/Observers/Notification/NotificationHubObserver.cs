using MassTransit;
using Serilog;
using Anemoi.Orchestration.Contract.NotificationContract.Instances;

namespace Anemoi.Orchestrator.Machine.Observers.Notification;

public class NotificationHubObserver(ILogger logger) : IStateObserver<NotificationHubInstance>
{
    public Task StateChanged(BehaviorContext<NotificationHubInstance> context, State currentState,
        State previousState)
    {
        logger.Information(
            "[NotificationHubObserver] has changed State from: {@PreviousState} to the new: {@NewState} for Saga: {@Saga}",
            previousState?.Name, currentState?.Name, context.Saga);
        return Task.CompletedTask;
    }
}