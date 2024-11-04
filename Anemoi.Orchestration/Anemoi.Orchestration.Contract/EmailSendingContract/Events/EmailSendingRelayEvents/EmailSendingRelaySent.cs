namespace Anemoi.Orchestration.Contract.EmailSendingContract.Events.EmailSendingRelayEvents;

public sealed record EmailSendingRelaySent
{
    public Guid CorrelationId { get; set; }
    public string Router { get; set; }
    public string EmailTo { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}