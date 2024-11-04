using MassTransit;

namespace Anemoi.Orchestration.Contract.WorkspaceContract.Instances;

public sealed class MemberRoleGroupSynchronizedInstance : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string WorkspaceId { get; set; }
    public string UserId { get; set; }
    public string MemberId { get; set; }
    public List<string> RoleGroupIds { get; set; }
    public string State { get; set; }
    public Guid? MonitorTimeoutTokenId { get; set; }
    public int Version { get; set; }
}