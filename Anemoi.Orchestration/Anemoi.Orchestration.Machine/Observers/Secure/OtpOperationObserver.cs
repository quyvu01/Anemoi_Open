using MassTransit;
using Serilog;
using Anemoi.Orchestration.Contract.SecureContract.Instances;

namespace Anemoi.Orchestrator.Machine.Observers.Secure;

public class OtpOperationObserver(ILogger logger) : IStateObserver<OtpOperationInstance>
{
    public Task StateChanged(BehaviorContext<OtpOperationInstance> context, State currentState,
        State previousState)
    {
        logger.Information(
            "[OtpOperationObserver] has changed State from: {@PreviousState} to the new: {@NewState} for Saga: {@Saga}",
            previousState?.Name, currentState?.Name, context.Saga);
        return Task.CompletedTask;
    }
}