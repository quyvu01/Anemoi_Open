using MassTransit;

namespace Anemoi.Orchestration.Contract.SecureContract.Instances;

public class OtpOperationInstance : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string PhoneNumber { get; set; }
    public string State { get; set; }
    public Guid? MonitorTimeoutTokenId { get; set; }
    public DateTime LastRecordTime { get; set; }
    public int Version { get; set; }
}