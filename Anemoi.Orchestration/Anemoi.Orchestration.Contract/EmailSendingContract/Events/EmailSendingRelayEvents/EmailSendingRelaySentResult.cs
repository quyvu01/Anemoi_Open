namespace Anemoi.Orchestration.Contract.EmailSendingContract.Events.EmailSendingRelayEvents;

public sealed record EmailSendingRelaySentResult
{
    public Guid CorrelationId { get; set; }
    public bool IsSucceed { get; set; }
}