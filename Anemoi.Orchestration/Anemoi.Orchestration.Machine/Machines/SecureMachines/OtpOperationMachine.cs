using AutoMapper;
using MassTransit;
using Serilog;
using Anemoi.Orchestration.Contract.SecureContract.Events.OtpCreationEvents;
using Anemoi.Orchestration.Contract.SecureContract.Instances;
using Anemoi.Orchestrator.Machine.Activities.SecureActivities.OtpOperationActivities;
using Anemoi.Orchestrator.Machine.Observers.Secure;

namespace Anemoi.Orchestrator.Machine.Machines.SecureMachines;

public sealed class OtpOperationMachine : MassTransitStateMachine<OtpOperationInstance>
{
    public State Sending { get; set; }
    public State ReadyToSend { get; set; }
    public Event<CreatePhoneNumberOtp> CreatePhoneNumberOtp { get; private set; }
    public Schedule<OtpOperationInstance, PhoneNumberOtpSentMonitor> PhoneNumberOtpSentMonitor { get; set; }

    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public OtpOperationMachine(ILogger logger, IMapper mapper)
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
        _logger.Information("Initialized Events for {@StateMachine}", nameof(OtpOperationMachine));
        Event(() => CreatePhoneNumberOtp, x => x
            .CorrelateBy((saga, ctx) => saga.PhoneNumber == ctx.Message.PhoneNumber)
            .SelectId(_ => NewId.NextGuid()));

        Schedule(() => PhoneNumberOtpSentMonitor, x => x.MonitorTimeoutTokenId, x =>
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
        _logger.Information("Initialized Event Behaviors for {@StateMachine}", nameof(OtpOperationMachine));
        During([Initial, ReadyToSend], When(CreatePhoneNumberOtp)
            .Then(ctx => _logger.Information("[CreatePhoneNumberOtp] message: {@Message}", ctx.Message))
            .Then(ctx => _mapper.Map(ctx.Message, ctx.Saga))
            .Schedule(PhoneNumberOtpSentMonitor,
                x => new PhoneNumberOtpSentMonitor { CorrelationId = x.Saga.CorrelationId })
            .TransitionTo(Sending)
            .RespondAsync(ctx => ctx.Init<OtpSendingState>(ctx.Saga))
            .Activity(c => c.OfType<CreatePhoneNumberOtpActivity>()));

        During(Sending, When(PhoneNumberOtpSentMonitor.Received)
            .Then(ctx =>
                _logger.Information("[PhoneNumberOtpSentMonitor.Received] ready to resend: {@Saga}", ctx.Saga))
            .Unschedule(PhoneNumberOtpSentMonitor)
            .TransitionTo(ReadyToSend));

        During(Sending, When(CreatePhoneNumberOtp)
            .Then(ctx =>
                _logger.Information("[CreatePhoneNumberOtp] Sending is not exceed timed!: {@Message}", ctx.Message))
            .RespondAsync(ctx => ctx.Init<OtpSendingState>(ctx.Saga)));
        SetCompletedWhenFinalized();
    }

    private void InitializeStateObserver() => ConnectStateObserver(new OtpOperationObserver(_logger));
}