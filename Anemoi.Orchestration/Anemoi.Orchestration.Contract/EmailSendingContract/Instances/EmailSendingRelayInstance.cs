using MassTransit;

namespace Anemoi.Orchestration.Contract.EmailSendingContract.Instances;

public class EmailSendingRelayInstance : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string Router { get; set; }
    public string EmailTo { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
    public TimeSpan? Timeout { get; set; }
    public TimeSpan[] RetryPeriods { get; set; }
    public int RetryPeriodIndex { get; set; }
    public string State { get; set; }
    public Guid? MonitorTimeoutTokenId { get; set; }
    public Guid? MonitorRetryTokenId { get; set; }
    public int Version { get; set; }
}