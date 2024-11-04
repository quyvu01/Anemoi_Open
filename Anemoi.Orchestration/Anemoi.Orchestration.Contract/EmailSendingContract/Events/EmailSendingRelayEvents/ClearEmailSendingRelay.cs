namespace Anemoi.Orchestration.Contract.EmailSendingContract.Events.EmailSendingRelayEvents;

public sealed record ClearEmailSendingRelay
{
    public Guid CorrelationId { get; set; }
}