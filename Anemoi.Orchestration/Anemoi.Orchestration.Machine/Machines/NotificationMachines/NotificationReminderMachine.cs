using AutoMapper;
using MassTransit;
using Serilog;
using Anemoi.Orchestration.Contract.NotificationContract.Events.NotificationHubEvents;
using Anemoi.Orchestration.Contract.NotificationContract.Events.NotificationReminderEvents;
using Anemoi.Orchestration.Contract.NotificationContract.Instances;
using Anemoi.Orchestrator.Machine.Observers.Notification;

namespace Anemoi.Orchestrator.Machine.Machines.NotificationMachines;

public sealed class NotificationReminderMachine : MassTransitStateMachine<NotificationReminderInstance>
{
    public State Scheduled { get; set; }
    public Event<CreateNotificationReminder> CreateNotificationReminderEvent { get; private set; }
    public Event<ClearNotificationReminder> ClearNotificationReminder { get; private set; }

    public Schedule<NotificationReminderInstance, NotificationReminderMonitoring>
        NotificationReminderMonitoring { get; set; }

    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public NotificationReminderMachine(ILogger logger, IMapper mapper)
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
        _logger.Information("Initialized Events for {@StateMachine}", nameof(NotificationReminderMachine));
        Event(() => CreateNotificationReminderEvent, x => x
            .CorrelateBy((_, _) => false).SelectId(_ => NewId.NextGuid()));

        Event(() => ClearNotificationReminder, x => x
            .CorrelateBy((saga, ctx) => saga.CorrelationData == ctx.Message.CorrelationData));

        Schedule(() => NotificationReminderMonitoring, x => x.MonitorTimeoutTokenId, x =>
        {
            x.DelayProvider = ctx => ctx.Saga.DeferAfter;
            x.Received = config =>
            {
                config.ConfigureConsumeTopology = false;
                config.CorrelateById(ctx => ctx.Message.CorrelationId);
            };
        });
    }

    private void InitBehaviors()
    {
        _logger.Information("Initialized Event Behaviors for {@StateMachine}",
            nameof(NotificationReminderMachine));
        Initially(When(CreateNotificationReminderEvent)
            .Then(ctx => _logger.Information("[CreateNotificationReminderEvent] message: {@Message}", ctx.Message))
            .Then(ctx => _mapper.Map(ctx.Message, ctx.Saga))
            .Schedule(NotificationReminderMonitoring,
                x => new NotificationReminderMonitoring { CorrelationId = x.Saga.CorrelationId })
            .TransitionTo(Scheduled));

        During(Scheduled, When(NotificationReminderMonitoring.Received)
            .Then(ctx =>
                _logger.Information("[NotificationReminderMonitoring.Received] ready to sent: {@Saga}", ctx.Saga))
            .Unschedule(NotificationReminderMonitoring)
            .PublishAsync(ctx => ctx.Init<CreateNotification>(ctx.Saga))
            .Finalize());

        DuringAny(When(ClearNotificationReminder)
            .Then(ctx =>
                _logger.Information("[ClearNotificationReminder] message: {@Message}", ctx.Message))
            .Unschedule(NotificationReminderMonitoring)
            .Finalize());

        SetCompletedWhenFinalized();
    }

    private void InitializeStateObserver() => ConnectStateObserver(new NotificationReminderObserver(_logger));
}