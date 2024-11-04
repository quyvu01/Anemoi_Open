using AutoMapper;
using MassTransit;
using Serilog;
using Anemoi.Orchestration.Contract.NotificationContract.Events.NotificationHubEvents;
using Anemoi.Orchestration.Contract.NotificationContract.Instances;
using Anemoi.Orchestrator.Machine.Observers.Notification;

namespace Anemoi.Orchestrator.Machine.Machines.NotificationMachines;

public sealed class NotificationHubMachine : MassTransitStateMachine<NotificationHubInstance>
{
    public State Sending { get; set; }
    public State SendingTimeout { get; set; }
    public State SendingFailed { get; set; }
    public Event<CreateNotification> CreateNotification { get; private set; }
    public Event<NotificationSentResult> NotificationSentResult { get; private set; }
    public Schedule<NotificationHubInstance, NotificationSentMonitoring> NotificationSentMonitoring { get; set; }

    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public NotificationHubMachine(ILogger logger, IMapper mapper)
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
        _logger.Information("Initialized Events for {@StateMachine}", nameof(NotificationHubMachine));
        Event(() => CreateNotification, x => x
            .CorrelateBy((_, _) => false).SelectId(_ => NewId.NextGuid()));

        Event(() => NotificationSentResult, x => x
            .CorrelateById(ctx => ctx.Message.CorrelationId));

        Schedule(() => NotificationSentMonitoring, x => x.MonitorTimeoutTokenId, x =>
        {
            x.Delay = TimeSpan.FromMinutes(2);
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
            nameof(NotificationHubMachine));
        During([Initial, SendingTimeout, SendingFailed], When(CreateNotification)
            .Then(ctx => _logger.Information("[CreateNotification] message: {@Message}", ctx.Message))
            .Then(ctx => _mapper.Map(ctx.Message, ctx.Saga))
            .Schedule(NotificationSentMonitoring,
                x => new NotificationSentMonitoring { CorrelationId = x.Saga.CorrelationId })
            .TransitionTo(Sending)
            .PublishAsync(ctx => ctx.Init<NotificationSent>(ctx.Saga)));

        During(Sending, When(NotificationSentMonitoring.Received)
            .Then(ctx =>
                _logger.Information(
                    "[NotificationSentMonitoring.Received] timeout while sending email: {@Saga}", ctx.Saga))
            .Unschedule(NotificationSentMonitoring)
            .TransitionTo(SendingTimeout), When(NotificationSentResult)
            .Then(ctx => _logger.Information("[NotificationSentResult] message: {@Message}", ctx.Message))
            .Unschedule(NotificationSentMonitoring)
            .IfElse(ctx => ctx.Message.IsSucceed, succeed => succeed.Finalize(),
                otherwise => otherwise.TransitionTo(SendingFailed)));
        
        SetCompletedWhenFinalized();
    }

    private void InitializeStateObserver() => ConnectStateObserver(new NotificationHubObserver(_logger));
}