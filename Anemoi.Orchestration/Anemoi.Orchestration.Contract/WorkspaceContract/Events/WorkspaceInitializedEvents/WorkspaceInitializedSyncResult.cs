namespace Anemoi.Orchestration.Contract.WorkspaceContract.Events.WorkspaceInitializedEvents;

public sealed record WorkspaceInitializedSyncResult
{
    public Guid CorrelationId { get; set; }
    public bool IsSucceed { get; set; }
    public string RoleGroupId { get; set; }
    public string UserId { get; set; }
}