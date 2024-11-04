namespace Anemoi.Orchestration.Contract.NotificationContract.Events.NotificationReminderEvents;

public sealed record CreateNotificationReminder
{
    public string NotificationType { get; set; }
    public string CorrelationData { get; set; }
    public string WorkspaceId { get; set; }
    public TimeSpan DeferAfter { get; set; }
    public List<string> TargetUserIds { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}