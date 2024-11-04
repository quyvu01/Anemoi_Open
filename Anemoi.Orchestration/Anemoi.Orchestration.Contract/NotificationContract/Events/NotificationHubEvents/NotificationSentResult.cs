namespace Anemoi.Orchestration.Contract.NotificationContract.Events.NotificationHubEvents;

public sealed record NotificationSentResult
{
    public Guid CorrelationId { get; set; }
    public bool IsSucceed { get; set; }
}