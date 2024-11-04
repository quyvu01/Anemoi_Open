using Anemoi.Orchestration.Contract.WorkspaceContract.Instances;
using MassTransit;
using Serilog;

namespace Anemoi.Orchestrator.Machine.Observers.Workspace;

public class WorkspaceInitializedObserver(ILogger logger) : IStateObserver<WorkspaceInitializedInstance>
{
    public Task StateChanged(BehaviorContext<WorkspaceInitializedInstance> context, State currentState,
        State previousState)
    {
        logger.Information(
            "[WorkspaceInitializedObserver] has changed State from: {@PreviousState} to the new: {@NewState} for Saga: {@Saga}",
            previousState?.Name, currentState?.Name, context.Saga);
        return Task.CompletedTask;
    }
}