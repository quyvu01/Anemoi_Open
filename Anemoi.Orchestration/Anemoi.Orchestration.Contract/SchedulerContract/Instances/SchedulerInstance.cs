using MassTransit;

namespace Anemoi.Orchestration.Contract.SchedulerContract.Instances;

public sealed class SchedulerInstance : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public Guid? MonitorTimeoutTokenId { get; set; }
    public TimeSpan SchedulerTime { get; set; }
    public string State { get; set; }
    public int Version { get; set; }
}