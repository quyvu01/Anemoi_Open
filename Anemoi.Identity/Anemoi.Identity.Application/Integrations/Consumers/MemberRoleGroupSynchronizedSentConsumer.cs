using System;
using System.Linq;
using System.Threading.Tasks;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Identity.Application.Cqrs.Commands.UserMapRoleGroupCommands.UpdateUserMapRoleGroups;
using Anemoi.Orchestration.Contract.WorkspaceContract.Events.MemberRoleGroupSynchronizedEvents;
using MassTransit;
using MediatR;
using Serilog;

namespace Anemoi.Identity.Application.Integrations.Consumers;

public sealed class MemberRoleGroupSynchronizedSentConsumer(
    ILogger logger,
    ISender sender) : IConsumer<MemberRoleGroupSynchronizedSent>
{
    public async Task Consume(ConsumeContext<MemberRoleGroupSynchronizedSent> context)
    {
        var message = context.Message;
        logger.Information("[MemberRoleGroupSynchronizedSent] message: {@@Message}", message);
        if (message.RoleGroupIds is not { Count: > 0 } || message.UserId is null || message.WorkspaceId is null) return;
        await sender.Send(new UpdateUserMapRoleGroupsCommand(new UserId(Guid.Parse(message.UserId)),
                message.RoleGroupIds.Select(a => new RoleGroupId(Guid.Parse(a))).ToList()),
            context.CancellationToken);
        await context.Publish<MemberRoleGroupSynchronizedSentResult>(new { message.CorrelationId, IsSucceed = true });
    }
}