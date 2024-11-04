namespace Anemoi.Orchestration.Contract.SecureContract.Events.OtpCreationEvents;

public sealed record CreatePhoneNumberOtp
{
    public string PhoneNumber { get; set; }
}