using MassTransit;

namespace Anemoi.Orchestration.Contract.WorkspaceContract.Instances;

public class WorkspaceInitializedInstance : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string UserId { get; set; }
    public string State { get; set; }
    public int Version { get; set; }
}