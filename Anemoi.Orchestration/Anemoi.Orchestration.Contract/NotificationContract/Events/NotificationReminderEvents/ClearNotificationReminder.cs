namespace Anemoi.Orchestration.Contract.NotificationContract.Events.NotificationReminderEvents;

public sealed record ClearNotificationReminder
{
    public string CorrelationData { get; set; }
}