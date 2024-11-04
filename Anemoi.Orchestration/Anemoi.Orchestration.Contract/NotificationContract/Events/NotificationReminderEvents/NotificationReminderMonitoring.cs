namespace Anemoi.Orchestration.Contract.NotificationContract.Events.NotificationReminderEvents;

public sealed record NotificationReminderMonitoring
{
    public Guid CorrelationId { get; set; }
}