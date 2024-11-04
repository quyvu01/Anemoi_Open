namespace Anemoi.Orchestration.Contract.SchedulerContract.Events.Scheduler;

public sealed record CreateScheduler
{
    public Guid CorrelationId { get; set; }
    public TimeSpan SchedulerTime { get; set; }
}