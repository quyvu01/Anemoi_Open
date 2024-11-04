namespace Anemoi.Orchestration.Contract.WorkspaceContract.Events.WorkspaceInitializedEvents;

public sealed record CreateWorkspaceInitialized
{
    public Guid WorkspaceId { get; set; }
    public string UserId { get; set; }
}