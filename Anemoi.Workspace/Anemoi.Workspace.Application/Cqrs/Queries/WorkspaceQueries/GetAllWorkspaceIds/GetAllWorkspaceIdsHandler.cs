using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetAllWorkspaceIds;
using Anemoi.Contract.Workspace.Responses;
using AutoMapper;
using Serilog;

namespace Anemoi.Workspace.Application.Cqrs.Queries.WorkspaceQueries.GetAllWorkspaceIds;

public sealed class GetAllWorkspaceIdsHandler(
    ISqlRepository<Anemoi.Workspace.Domain.Models.Workspace> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryCollectionHandler<Anemoi.Workspace.Domain.Models.Workspace,
        GetAllWorkspaceIdsQuery, WorkspaceIdResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryListFlowBuilder<Anemoi.Workspace.Domain.Models.Workspace, WorkspaceIdResponse>
        BuildQueryFlow(
            IQueryListFilter<Anemoi.Workspace.Domain.Models.Workspace, WorkspaceIdResponse> fromFlow,
            GetAllWorkspaceIdsQuery query)
        => fromFlow
            .WithFilter(null)
            .WithSpecialAction(a => a.Select(x => new WorkspaceIdResponse { Id = x.Id.ToString() }))
            .WithSortFieldWhenNotSet(a => a.Id)
            .WithSortedDirectionWhenNotSet(SortedDirection.Ascending);
}