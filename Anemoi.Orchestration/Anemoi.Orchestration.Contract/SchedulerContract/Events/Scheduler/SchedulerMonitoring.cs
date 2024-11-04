namespace Anemoi.Orchestration.Contract.SchedulerContract.Events.Scheduler;

public sealed record SchedulerMonitoring
{
    public Guid CorrelationId { get; set; }
}