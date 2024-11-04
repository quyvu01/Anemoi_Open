using MassTransit;
using Serilog;
using Anemoi.Orchestration.Contract.NotificationContract.Instances;

namespace Anemoi.Orchestrator.Machine.Observers.Notification;

public class NotificationReminderObserver(ILogger logger) : IStateObserver<NotificationReminderInstance>
{
    public Task StateChanged(BehaviorContext<NotificationReminderInstance> context, State currentState,
        State previousState)
    {
        logger.Information(
            "[NotificationReminderObserver] has changed State from: {@PreviousState} to the new: {@NewState} for Saga: {@Saga}",
            previousState?.Name, currentState?.Name, context.Saga);
        return Task.CompletedTask;
    }
}