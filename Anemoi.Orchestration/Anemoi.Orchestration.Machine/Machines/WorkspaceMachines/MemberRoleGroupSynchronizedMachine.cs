using Anemoi.Orchestration.Contract.WorkspaceContract.Events.MemberRoleGroupSynchronizedEvents;
using Anemoi.Orchestration.Contract.WorkspaceContract.Instances;
using Anemoi.Orchestrator.Machine.Observers.Workspace;
using AutoMapper;
using MassTransit;
using Serilog;

namespace Anemoi.Orchestrator.Machine.Machines.WorkspaceMachines;

public sealed class MemberRoleGroupSynchronizedMachine : MassTransitStateMachine<MemberRoleGroupSynchronizedInstance>
{
    public State Sending { get; set; }
    public State SendingTimeout { get; set; }
    public State SendingFailed { get; set; }
    public Event<CreateMemberRoleGroupSynchronized> CreateMemberRoleGroupSynchronized { get; private set; }
    public Event<MemberRoleGroupSynchronizedSentResult> MemberRoleGroupSynchronizedSentResult { get; private set; }

    public Schedule<MemberRoleGroupSynchronizedInstance, MemberRoleGroupSynchronizedSentMonitoring>
        MemberRoleGroupSynchronizedSentMonitoring { get; set; }

    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public MemberRoleGroupSynchronizedMachine(ILogger logger, IMapper mapper)
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
        _logger.Information("Initialized Events for {@StateMachine}", nameof(MemberRoleGroupSynchronizedMachine));
        Event(() => CreateMemberRoleGroupSynchronized, x => x
            .CorrelateBy((_, _) => false).SelectId(_ => NewId.NextGuid()));

        Event(() => MemberRoleGroupSynchronizedSentResult, x => x
            .CorrelateById(ctx => ctx.Message.CorrelationId));

        Schedule(() => MemberRoleGroupSynchronizedSentMonitoring, x => x.MonitorTimeoutTokenId, x =>
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
            nameof(MemberRoleGroupSynchronizedMachine));
        During([Initial, SendingTimeout, SendingFailed], When(CreateMemberRoleGroupSynchronized)
            .Then(ctx => _logger.Information("[CreateMemberRoleGroupSynchronized] message: {@Message}", ctx.Message))
            .Then(ctx => _mapper.Map(ctx.Message, ctx.Saga))
            .Schedule(MemberRoleGroupSynchronizedSentMonitoring,
                x => new MemberRoleGroupSynchronizedSentMonitoring { CorrelationId = x.Saga.CorrelationId })
            .TransitionTo(Sending)
            .PublishAsync(ctx => ctx.Init<MemberRoleGroupSynchronizedSent>(ctx.Saga)));

        During(Sending, When(MemberRoleGroupSynchronizedSentMonitoring.Received)
            .Then(ctx =>
                _logger.Information(
                    "[MemberRoleGroupSynchronizedSentMonitoring.Received] timeout while sending email: {@Saga}",
                    ctx.Saga))
            .Unschedule(MemberRoleGroupSynchronizedSentMonitoring)
            .TransitionTo(SendingTimeout), When(MemberRoleGroupSynchronizedSentResult)
            .Then(ctx =>
                _logger.Information("[MemberRoleGroupSynchronizedSentResult] message: {@Message}", ctx.Message))
            .Unschedule(MemberRoleGroupSynchronizedSentMonitoring)
            .IfElse(ctx => ctx.Message.IsSucceed, succeed => succeed.Finalize(),
                otherwise => otherwise.TransitionTo(SendingFailed)));

        SetCompletedWhenFinalized();
    }

    private void InitializeStateObserver() => ConnectStateObserver(new MemberRoleGroupSynchronizedObserver(_logger));
}