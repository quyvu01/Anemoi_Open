namespace Anemoi.Orchestration.Contract.EmailSendingContract.Events.EmailSendingRelayEvents;

public sealed record CreateEmailSendingRelay
{
    public Guid CorrelationId { get; set; }
    public string Router { get; set; }
    public string EmailTo { get; set; }
    public TimeSpan? Timeout { get; set; }
    public TimeSpan[] RetryPeriods { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}