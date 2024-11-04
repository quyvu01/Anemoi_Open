namespace Anemoi.Orchestration.Contract.WorkspaceContract.Events.MemberRoleGroupSynchronizedEvents;

public sealed record MemberRoleGroupSynchronizedSent
{
    public Guid CorrelationId { get; set; }
    public string WorkspaceId { get; set; }
    public string UserId { get; set; }
    public string MemberId { get; set; }
    public List<string> RoleGroupIds { get; set; }
}