namespace Anemoi.Orchestration.Contract.SecureContract.Events.OtpCreationEvents;

public sealed record PhoneNumberOtpSentMonitor
{
    public Guid CorrelationId { get; set; }
}