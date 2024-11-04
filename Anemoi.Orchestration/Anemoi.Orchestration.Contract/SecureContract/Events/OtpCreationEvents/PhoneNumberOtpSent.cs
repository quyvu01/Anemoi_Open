namespace Anemoi.Orchestration.Contract.SecureContract.Events.OtpCreationEvents;

public sealed record PhoneNumberOtpSent
{
    public Guid CorrelationId { get; set; }
    public string PhoneNumber { get; set; }
    public string Otp { get; set; }
}