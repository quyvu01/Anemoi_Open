using Anemoi.Orchestration.Contract.WorkspaceContract.Events.WorkspaceInitializedEvents;
using Anemoi.Orchestration.Contract.WorkspaceContract.Instances;
using Anemoi.Orchestrator.Machine.Observers.Workspace;
using AutoMapper;
using MassTransit;
using Serilog;

namespace Anemoi.Orchestrator.Machine.Machines.WorkspaceMachines;

public class WorkspaceInitializedMachine : MassTransitStateMachine<WorkspaceInitializedInstance>
{
    public State Processing { get; set; }
    public State ProcessingFailed { get; set; }
    public Event<CreateWorkspaceInitialized> CreateWorkspaceInitialized { get; private set; }
    public Event<WorkspaceInitializedSentResult> WorkspaceInitializedSentResult { get; private set; }
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public WorkspaceInitializedMachine(ILogger logger, IMapper mapper)
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
        _logger.Information("Initialized Events for {@StateMachine}", nameof(WorkspaceInitializedMachine));
        Event(() => CreateWorkspaceInitialized, x => x
            .CorrelateById(ctx => ctx.Message.WorkspaceId).SelectId(ctx => ctx.Message.WorkspaceId));

        Event(() => WorkspaceInitializedSentResult, x => x
            .CorrelateById(ctx => ctx.Message.CorrelationId));
    }

    private void InitBehaviors()
    {
        _logger.Information("Initialized Event Behaviors for {@StateMachine}", nameof(WorkspaceInitializedMachine));
        Initially(When(CreateWorkspaceInitialized)
            .Then(ctx => _logger.Information("[CreateWorkspaceInitialized] message: {@Message}", ctx.Message))
            .Then(ctx => _mapper.Map(ctx.Message, ctx.Saga))
            .PublishAsync(ctx => ctx.Init<WorkspaceInitializedSent>(ctx.Saga))
            .TransitionTo(Processing));

        During(Processing, When(WorkspaceInitializedSentResult)
            .Then(ctx => _logger.Information("[WorkspaceInitializedSentResult] message: {@Message}", ctx.Message))
            .IfElse(ctx => ctx.Message.IsSucceed,
                succeed => succeed
                    .PublishAsync(ctx => ctx
                        .Init<WorkspaceInitializedSyncResult>(new
                        {
                            ctx.Saga.CorrelationId, IsSucceed = true,
                            ctx.Message.RoleGroupId, ctx.Saga.UserId
                        }))
                    .Finalize(),
                otherwise => otherwise
                    .PublishAsync(ctx => ctx
                        .Init<WorkspaceInitializedSyncResult>(new { ctx.Saga.CorrelationId, IsSucceed = false }))
                    .TransitionTo(ProcessingFailed)));

        SetCompletedWhenFinalized();
    }

    private void InitializeStateObserver() => ConnectStateObserver(new WorkspaceInitializedObserver(_logger));
}