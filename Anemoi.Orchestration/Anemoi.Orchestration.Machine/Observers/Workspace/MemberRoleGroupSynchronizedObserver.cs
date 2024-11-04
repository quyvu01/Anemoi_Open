using Anemoi.Orchestration.Contract.WorkspaceContract.Instances;
using MassTransit;
using Serilog;

namespace Anemoi.Orchestrator.Machine.Observers.Workspace;

public class MemberRoleGroupSynchronizedObserver(ILogger logger) : IStateObserver<MemberRoleGroupSynchronizedInstance>
{
    public Task StateChanged(BehaviorContext<MemberRoleGroupSynchronizedInstance> context, State currentState,
        State previousState)
    {
        logger.Information(
            "[MemberRoleGroupSynchronizedObserver] has changed State from: {@PreviousState} to the new: {@NewState} for Saga: {@Saga}",
            previousState?.Name, currentState?.Name, context.Saga);
        return Task.CompletedTask;
    }
}