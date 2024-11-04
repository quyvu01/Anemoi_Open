namespace Anemoi.Orchestration.Contract.SecureContract.Events.OtpCreationEvents;

public sealed record OtpSendingState
{
    public string State { get; set; }
    public DateTime LastRecordTime { get; set; }
}