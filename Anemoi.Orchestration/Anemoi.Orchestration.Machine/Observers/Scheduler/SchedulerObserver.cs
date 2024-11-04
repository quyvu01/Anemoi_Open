using MassTransit;
using Serilog;
using Anemoi.Orchestration.Contract.SchedulerContract.Instances;

namespace Anemoi.Orchestrator.Machine.Observers.Scheduler;

public class SchedulerObserver(ILogger logger) : IStateObserver<SchedulerInstance>
{
    public Task StateChanged(BehaviorContext<SchedulerInstance> context, State currentState,
        State previousState)
    {
        logger.Information(
            "[SchedulerObserver] has changed State from: {@PreviousState} to the new: {@NewState} for Saga: {@Saga}",
            previousState?.Name, currentState?.Name, context.Saga);
        return Task.CompletedTask;
    }
}