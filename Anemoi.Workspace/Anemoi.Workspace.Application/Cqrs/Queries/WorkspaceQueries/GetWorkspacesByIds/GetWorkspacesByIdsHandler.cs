using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetWorkspacesByIds;
using Anemoi.Contract.Workspace.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;

namespace Anemoi.Workspace.Application.Cqrs.Queries.WorkspaceQueries.GetWorkspacesByIds;

public sealed class GetWorkspacesByIdsHandler(
    ISqlRepository<Anemoi.Workspace.Domain.Models.Workspace> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryCollectionHandler<Anemoi.Workspace.Domain.Models.Workspace, GetWorkspacesByIdsQuery, WorkspaceResponse>(
        sqlRepository, mapper, logger)
{
    protected override IQueryListFlowBuilder<Anemoi.Workspace.Domain.Models.Workspace, WorkspaceResponse>
        BuildQueryFlow(
            IQueryListFilter<Anemoi.Workspace.Domain.Models.Workspace, WorkspaceResponse> fromFlow,
            GetWorkspacesByIdsQuery query)
        => fromFlow
            .WithFilter(a => query.Ids.Contains(a.Id))
            .WithSpecialAction(a => a.ProjectTo<WorkspaceResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(a => a.Id)
            .WithSortedDirectionWhenNotSet(SortedDirection.Ascending);
}