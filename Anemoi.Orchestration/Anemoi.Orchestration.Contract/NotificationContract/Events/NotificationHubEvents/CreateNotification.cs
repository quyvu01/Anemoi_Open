namespace Anemoi.Orchestration.Contract.NotificationContract.Events.NotificationHubEvents;

public sealed record CreateNotification
{
    public string NotificationType { get; set; }
    public string WorkspaceId { get; set; }
    public List<string> TargetUserIds { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}