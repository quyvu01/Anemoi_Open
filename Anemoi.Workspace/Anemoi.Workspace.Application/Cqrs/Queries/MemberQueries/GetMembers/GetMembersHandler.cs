using System.Linq.Expressions;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Queries.MemberQueries.GetMembers;
using Anemoi.Contract.Workspace.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Cqrs.Queries.MemberQueries.GetMembers;

public sealed class GetMembersHandler(
    ISqlRepository<Member> sqlRepository,
    IMapper mapper,
    ILogger logger,
    IWorkspaceIdGetter workspaceIdGetter)
    : EfQueryPaginationHandler<Member, GetMembersQuery, MemberResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryListFlowBuilder<Member, MemberResponse> BuildQueryFlow(
        IQueryListFilter<Member, MemberResponse> fromFlow, GetMembersQuery query)
    {
        Expression<Func<Member, bool>> userFilter = query.UserIds switch
        {
            { } val => a => val.Contains(a.UserId),
            _ => _ => true
        };

        Expression<Func<Member, bool>> activeFilter = query.IsActivated switch
        {
            { } val => m => m.IsActivated == val,
            _ => _ => true
        };
        Expression<Func<Member, bool>> removedFilter = query.IsRemoved switch
        {
            { } val => m => m.IsRemoved == val,
            _ => _ => true
        };

        var finalFilter = ExpressionHelper.CombineAnd(userFilter, activeFilter, removedFilter,
            x => x.WorkspaceId == new WorkspaceId(Guid.Parse(workspaceIdGetter.WorkspaceId)));

        return fromFlow
            .WithFilter(finalFilter)
            .WithSpecialAction(a => a.ProjectTo<MemberResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(x => x.CreatedTime)
            .WithSortedDirectionWhenNotSet(SortedDirection.Descending);
    }
}