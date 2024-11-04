namespace Anemoi.Orchestration.Contract.WorkspaceContract.Events.WorkspaceInitializedEvents;

public sealed record WorkspaceInitializedSent
{
    public Guid CorrelationId { get; set; }
    public string UserId { get; set; }
}