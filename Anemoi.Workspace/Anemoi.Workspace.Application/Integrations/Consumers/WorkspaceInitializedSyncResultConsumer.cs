using Anemoi.Contract.Workspace.Commands.MemberMapRoleGroupCommands.UpdateMemberByUserId;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Orchestration.Contract.WorkspaceContract.Events.WorkspaceInitializedEvents;
using Anemoi.Workspace.Application.Cqrs.Commands.WorkspaceCommands.UpdateWorkspaceState;
using Anemoi.Workspace.Domain.Models.ValueTypes;
using MassTransit;
using MediatR;
using Serilog;

namespace Anemoi.Workspace.Application.Integrations.Consumers;

public sealed class WorkspaceInitializedSyncResultConsumer(ILogger logger, ISender sender) :
    IConsumer<WorkspaceInitializedSyncResult>
{
    public async Task Consume(ConsumeContext<WorkspaceInitializedSyncResult> context)
    {
        var message = context.Message;
        logger.Information("[WorkspaceInitializedSyncResult] message: {@Message}", message);
        var workspaceId = new WorkspaceId(message.CorrelationId);
        var workspaceState = message.IsSucceed ? WorkspaceState.Completed : WorkspaceState.SettingFailed;
        await sender.Send(new UpdateWorkspaceStateCommand(workspaceId, workspaceState),
            context.CancellationToken);
        await sender.Send(new UpdateMemberByUserIdCommand(message.UserId, workspaceId, [message.RoleGroupId]));
    }
}