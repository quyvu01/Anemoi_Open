namespace Anemoi.Orchestration.Contract.EmailSendingContract.Events.EmailSendingRelayEvents;

public sealed record ResendEmailSendingRelay
{
    public Guid CorrelationId { get; set; }
}