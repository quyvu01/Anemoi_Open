using Anemoi.Orchestration.Contract.EmailSendingContract.Instances;
using MassTransit;
using Serilog;

namespace Anemoi.Orchestrator.Machine.Observers.EmailSending;

public class EmailSendingRelayObserver(ILogger logger) : IStateObserver<EmailSendingRelayInstance>
{
    public Task StateChanged(BehaviorContext<EmailSendingRelayInstance> context, State currentState,
        State previousState)
    {
        logger.Information(
            "[EmailSendingRelayObserver] has changed State from: {@PreviousState} to the new: {@NewState} for Saga: {@Saga}",
            previousState?.Name, currentState?.Name, context.Saga);
        return Task.CompletedTask;
    }
}