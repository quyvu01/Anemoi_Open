namespace Anemoi.Orchestration.Contract.NotificationContract.Events.NotificationHubEvents;

public sealed record NotificationSentMonitoring
{
    public Guid CorrelationId { get; set; }
}