using AutoMapper;
using MassTransit;
using Serilog;
using Anemoi.Orchestration.Contract.SchedulerContract.Events.Scheduler;
using Anemoi.Orchestration.Contract.SchedulerContract.Instances;
using Anemoi.Orchestrator.Machine.Observers.Scheduler;

namespace Anemoi.Orchestrator.Machine.Machines.SchedulerMachines;

public class SchedulerMachine : MassTransitStateMachine<SchedulerInstance>
{
    public State Scheduled { get; set; }
    public Event<CreateScheduler> CreateScheduler { get; private set; }
    public Event<ClearScheduler> ClearScheduler { get; private set; }
    public Schedule<SchedulerInstance, SchedulerMonitoring> SchedulerMonitoring { get; set; }
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public SchedulerMachine(ILogger logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
        InitState();
        InitEvents();
        InitBehaviors();
        InitializeStateObserver();
    }

    private void InitState() => InstanceState(x => x.State);

    private void InitEvents()
    {
        _logger.Information("Initialized Events for {@StateMachine}", nameof(SchedulerMachine));
        Event(() => CreateScheduler, x => x
            .CorrelateById(ctx => ctx.Message.CorrelationId).SelectId(ctx => ctx.Message.CorrelationId));

        Event(() => ClearScheduler, x => x
            .CorrelateById(ctx => ctx.Message.CorrelationId));

        Schedule(() => SchedulerMonitoring, x => x.MonitorTimeoutTokenId, x =>
        {
            x.DelayProvider = ctx => ctx.Saga.SchedulerTime;
            x.Received = config =>
            {
                config.ConfigureConsumeTopology = false;
                config.CorrelateById(ctx => ctx.Message.CorrelationId);
            };
        });
    }

    private void InitBehaviors()
    {
        _logger.Information("Initialized Event Behaviors for {@StateMachine}", nameof(SchedulerMachine));
        Initially(When(CreateScheduler)
            .Then(ctx => _logger.Information("[CreateScheduler] message: {@Message}", ctx.Message))
            .Then(ctx => _mapper.Map(ctx.Message, ctx.Saga))
            .Schedule(SchedulerMonitoring,
                x => new SchedulerMonitoring { CorrelationId = x.Saga.CorrelationId })
            .TransitionTo(Scheduled));

        During(Scheduled, When(SchedulerMonitoring.Received)
            .Then(ctx =>
                _logger.Information("[CreateScheduler.Received] timed: {@Saga}", ctx.Saga))
            .Unschedule(SchedulerMonitoring)
            .PublishAsync(ctx => ctx.Init<SchedulerSent>(ctx.Saga))
            .Finalize(), When(ClearScheduler)
            .Then(ctx => _logger.Information("[ClearScheduler] message: {@Message}", ctx.Message))
            .Unschedule(SchedulerMonitoring)
            .Finalize());

        SetCompletedWhenFinalized();
    }

    private void InitializeStateObserver() => ConnectStateObserver(new SchedulerObserver(_logger));
}