namespace Anemoi.Orchestration.Contract.SchedulerContract.Events.Scheduler;

public sealed record ClearScheduler
{
    public Guid CorrelationId { get; set; }
}