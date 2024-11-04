using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Queries.MemberInvitationQueries.GetMemberInvitations;
using Anemoi.Contract.Workspace.Responses;
using Anemoi.Workspace.Domain.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;

namespace Anemoi.Workspace.Application.Cqrs.Queries.MemberInvitationQueries.GetMemberInvitations;

public sealed class GetMemberInvitationsHandler(
    ISqlRepository<MemberInvitation> sqlRepository,
    IWorkspaceIdGetter workspaceIdGetter,
    IMapper mapper,
    ILogger logger)
    : EfQueryPaginationHandler<MemberInvitation,
        GetMemberInvitationsQuery, MemberInvitationResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryListFlowBuilder<MemberInvitation, MemberInvitationResponse> BuildQueryFlow(
        IQueryListFilter<MemberInvitation, MemberInvitationResponse> fromFlow,
        GetMemberInvitationsQuery query)
        => fromFlow
            .WithFilter(x => x.WorkspaceId == new WorkspaceId(Guid.Parse(workspaceIdGetter.WorkspaceId)))
            .WithSpecialAction(x => x.ProjectTo<MemberInvitationResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(x => x.CreatedTime)
            .WithSortedDirectionWhenNotSet(SortedDirection.Descending);
}