namespace Anemoi.Orchestration.Contract.EmailSendingContract.Events.EmailSendingRelayEvents;

public sealed record EmailSendingRelayRetryMonitor
{
    public Guid CorrelationId { get; set; }
}