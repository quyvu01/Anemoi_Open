using AutoMapper;
using Anemoi.Orchestration.Contract.SchedulerContract.Events.Scheduler;
using Anemoi.Orchestration.Contract.SchedulerContract.Instances;

namespace Anemoi.Orchestrator.Machine.Mappings.SchedulerMappings;

public class ToSchedulerMapping : Profile
{
    public ToSchedulerMapping()
    {
        CreateMap<CreateScheduler, SchedulerInstance>();
    }
}