using System.Linq.Expressions;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetWorkspaces;
using Anemoi.Contract.Workspace.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;

namespace Anemoi.Workspace.Application.Cqrs.Queries.WorkspaceQueries.GetWorkspaces;

public sealed class GetWorkspacesHandler(
    ISqlRepository<Anemoi.Workspace.Domain.Models.Workspace> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryPaginationHandler<Anemoi.Workspace.Domain.Models.Workspace, GetWorkspacesQuery, WorkspaceResponse>(sqlRepository, mapper,
        logger)
{
    protected override IQueryListFlowBuilder<Anemoi.Workspace.Domain.Models.Workspace, WorkspaceResponse> BuildQueryFlow(
        IQueryListFilter<Anemoi.Workspace.Domain.Models.Workspace, WorkspaceResponse> fromFlow, GetWorkspacesQuery query)
        => fromFlow
            .WithFilter(BuildFilter(query))
            .WithSpecialAction(x => x.ProjectTo<WorkspaceResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(a => a.CreatedTime)
            .WithSortedDirectionWhenNotSet(SortedDirection.Descending);

    private static Expression<Func<Anemoi.Workspace.Domain.Models.Workspace, bool>> BuildFilter(GetWorkspacesQuery query)
    {
        var searchHint = query.SearchKey.GenerateSearchHint();
        Expression<Func<Anemoi.Workspace.Domain.Models.Workspace, bool>> searchHintFilter = searchHint switch
        {
            { } val => w => w.SearchHint.Contains(val),
            _ => _ => true
        };
        Expression<Func<Anemoi.Workspace.Domain.Models.Workspace, bool>>
            userFilter = x => x.UserId == query.UserId;
        return ExpressionHelper.CombineAnd(searchHintFilter, userFilter);
    }
}