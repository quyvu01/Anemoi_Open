using Anemoi.Orchestration.Contract.EmailSendingContract.Events.EmailSendingRelayEvents;
using Anemoi.Orchestration.Contract.EmailSendingContract.Instances;
using AutoMapper;
using MassTransit;
using Serilog;
using Anemoi.Orchestrator.Machine.Observers.EmailSending;

namespace Anemoi.Orchestrator.Machine.Machines.EmailSendingMachines;

public sealed class EmailSendingRelayMachine : MassTransitStateMachine<EmailSendingRelayInstance>
{
    private readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(20);
    public State Sending { get; private set; }
    public State SendingTimeout { get; private set; }
    public State Retrying { get; private set; }
    public State SendingFailed { get; private set; }
    public Event<CreateEmailSendingRelay> CreateEmailSendingRelay { get; private set; }
    public Event<ResendEmailSendingRelay> ResendEmailSendingRelay { get; private set; }
    public Event<ClearEmailSendingRelay> ClearEmailSendingRelay { get; private set; }
    public Event<EmailSendingRelaySentResult> EmailSendingRelaySentResult { get; private set; }

    public Schedule<EmailSendingRelayInstance, EmailSendingRelaySentMonitor>
        EmailSendingRelaySentMonitor { get; set; }

    public Schedule<EmailSendingRelayInstance, EmailSendingRelayRetryMonitor>
        EmailSendingRelayRetryMonitor { get; set; }

    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public EmailSendingRelayMachine(ILogger logger, IMapper mapper)
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
        _logger.Information("Initialized Events for {@StateMachine}", nameof(EmailSendingRelayMachine));
        Event(() => CreateEmailSendingRelay, x => x
            .CorrelateById(ctx => ctx.Message.CorrelationId).SelectId(ctx => ctx.Message.CorrelationId));

        Event(() => EmailSendingRelaySentResult, x => x
            .CorrelateById(ctx => ctx.Message.CorrelationId));

        Event(() => ResendEmailSendingRelay, x => x
            .CorrelateById(ctx => ctx.Message.CorrelationId));

        Event(() => ClearEmailSendingRelay, x => x
            .CorrelateById(ctx => ctx.Message.CorrelationId));

        Schedule(() => EmailSendingRelaySentMonitor, x => x.MonitorTimeoutTokenId, x =>
        {
            x.DelayProvider = ctx => ctx.Saga.Timeout ?? _defaultTimeout;
            x.Received = config =>
            {
                config.ConfigureConsumeTopology = false;
                config.CorrelateById(ctx => ctx.Message.CorrelationId);
            };
        });

        Schedule(() => EmailSendingRelayRetryMonitor, x => x.MonitorRetryTokenId, x =>
        {
            x.DelayProvider = ctx => ctx.Saga.RetryPeriods[ctx.Saga.RetryPeriodIndex];
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
            nameof(EmailSendingRelayMachine));
        During([Initial, SendingTimeout, SendingFailed], When(CreateEmailSendingRelay)
            .Then(ctx => _logger.Information("[CreateEmailSendingRelay] message: {@Message}", ctx.Message))
            .Then(ctx => _mapper.Map(ctx.Message, ctx.Saga))
            .Schedule(EmailSendingRelaySentMonitor,
                x => new EmailSendingRelaySentMonitor { CorrelationId = x.Saga.CorrelationId })
            .TransitionTo(Sending)
            .PublishAsync(ctx => ctx.Init<EmailSendingRelaySent>(ctx.Saga)));

        During(Sending, When(EmailSendingRelaySentMonitor.Received)
                .Then(ctx =>
                    _logger.Information(
                        "[EmailSendingRelaySentMonitor.Received] timeout while sending email: {@Saga}", ctx.Saga))
                .Unschedule(EmailSendingRelaySentMonitor)
                .IfElse(ctx => ctx.Saga.RetryPeriods is not { Length: > 0 }, notRetry => notRetry
                    .TransitionTo(SendingTimeout), retryBehavior => retryBehavior
                    .IfElse(ctx => ctx.Saga.RetryPeriodIndex > ctx.Saga.RetryPeriods.Length - 1,
                        exceedRetry => exceedRetry.TransitionTo(SendingTimeout), retryLess => retryLess
                            .Then(ctx => _logger.Information("[Retry for timeout behavior] : {@Saga}", ctx.Saga))
                            .Schedule(EmailSendingRelayRetryMonitor,
                                x => new EmailSendingRelayRetryMonitor { CorrelationId = x.Saga.CorrelationId })
                            .TransitionTo(Retrying))),
            When(EmailSendingRelaySentResult)
                .Then(ctx => _logger.Information("[EmailSendingRelaySentResult] message: {@Message}", ctx.Message))
                .Unschedule(EmailSendingRelaySentMonitor)
                .IfElse(ctx => ctx.Message.IsSucceed, succeed => succeed.Finalize(),
                    otherwise => otherwise
                        .IfElse(ctx => ctx.Saga.RetryPeriods is not { Length: > 0 }, notRetry => notRetry
                            .TransitionTo(SendingFailed), retryBehavior => retryBehavior
                            .IfElse(ctx => ctx.Saga.RetryPeriodIndex > ctx.Saga.RetryPeriods.Length - 1,
                                exceedRetry => exceedRetry.TransitionTo(SendingFailed), retryLess => retryLess
                                    .Then(ctx => _logger.Information("[Retry for failed behavior] : {@Saga}", ctx.Saga))
                                    .Schedule(EmailSendingRelayRetryMonitor,
                                        x => new EmailSendingRelayRetryMonitor { CorrelationId = x.Saga.CorrelationId })
                                    .TransitionTo(Retrying)))));

        During(Retrying, When(EmailSendingRelayRetryMonitor.Received)
            .Then(ctx => _logger.Information(
                "[EmailSendingRelayRetryMonitor.Received] executing retry for email sending relay: {@Saga}", ctx.Saga))
            .Unschedule(EmailSendingRelayRetryMonitor)
            .Then(ctx => ctx.Saga.RetryPeriodIndex += 1)
            .Unschedule(EmailSendingRelaySentMonitor)
            .Schedule(EmailSendingRelaySentMonitor,
                x => new EmailSendingRelaySentMonitor { CorrelationId = x.Saga.CorrelationId })
            .TransitionTo(Sending)
            .PublishAsync(ctx => ctx.Init<EmailSendingRelaySent>(ctx.Saga)));

        During([SendingTimeout, SendingFailed], When(ResendEmailSendingRelay)
            .Then(ctx => _logger.Information(
                "[ResendEmailSendingRelay] message: {@Message}", ctx.Message))
            .Unschedule(EmailSendingRelayRetryMonitor)
            .Unschedule(EmailSendingRelaySentMonitor)
            .Then(ctx => ctx.Saga.RetryPeriodIndex = 0)
            .Schedule(EmailSendingRelaySentMonitor,
                x => new EmailSendingRelaySentMonitor { CorrelationId = x.Saga.CorrelationId })
            .TransitionTo(Sending)
            .PublishAsync(ctx => ctx.Init<EmailSendingRelaySent>(ctx.Saga)));

        During([Sending, Retrying], Ignore(CreateEmailSendingRelay), Ignore(ResendEmailSendingRelay));

        DuringAny(When(ClearEmailSendingRelay)
            .Then(ctx => _logger.Information(
                "[ClearEmailSendingRelay] message: {@Message}", ctx.Message))
            .Unschedule(EmailSendingRelayRetryMonitor)
            .Unschedule(EmailSendingRelaySentMonitor)
            .Finalize());
        SetCompletedWhenFinalized();
    }

    private void InitializeStateObserver() => ConnectStateObserver(new EmailSendingRelayObserver(_logger));
}