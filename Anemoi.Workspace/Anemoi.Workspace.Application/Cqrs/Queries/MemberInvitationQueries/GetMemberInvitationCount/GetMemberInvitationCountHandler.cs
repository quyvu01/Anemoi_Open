using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.CountingFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryCounting;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Queries.MemberInvitationQueries.GetMemberInvitationCount;
using Serilog;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Cqrs.Queries.MemberInvitationQueries.GetMemberInvitationCount;

public sealed class GetMemberInvitationCountHandler(
    ISqlRepository<MemberInvitation> sqlRepository,
    ILogger logger,
    IWorkspaceIdGetter workspaceIdGetter)
    :
        EfQueryCountingHandler<MemberInvitation, GetMemberInvitationCountQuery>(sqlRepository, logger)
{
    protected override ICountingFlowBuilder<MemberInvitation> BuildQueryFlow(ICountingFilter<MemberInvitation> fromFlow,
        GetMemberInvitationCountQuery query)
        => fromFlow
            .WithFilter(a => a.WorkspaceId == new WorkspaceId(Guid.Parse(workspaceIdGetter.WorkspaceId)));
}