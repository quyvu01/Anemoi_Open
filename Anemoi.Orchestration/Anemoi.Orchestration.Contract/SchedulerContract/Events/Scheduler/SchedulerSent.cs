namespace Anemoi.Orchestration.Contract.SchedulerContract.Events.Scheduler;

public sealed record SchedulerSent
{
    public Guid CorrelationId { get; set; }
}