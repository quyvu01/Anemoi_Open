namespace Anemoi.Orchestration.Contract.WorkspaceContract.Events.MemberRoleGroupSynchronizedEvents;

public sealed record MemberRoleGroupSynchronizedSentResult
{
    public Guid CorrelationId { get; set; }
    public bool IsSucceed { get; set; }
}