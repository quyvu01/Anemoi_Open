namespace Anemoi.Orchestration.Contract.EmailSendingContract.Events.EmailSendingRelayEvents;

public record EmailSendingRelaySentMonitor
{
    public Guid CorrelationId { get; set; }
}