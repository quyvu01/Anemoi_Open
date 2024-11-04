using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetWorkspacesByUser;
using Anemoi.Contract.Workspace.Responses;
using Anemoi.Workspace.Domain.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;

namespace Anemoi.Workspace.Application.Cqrs.Queries.WorkspaceQueries.GetWorkspacesByUser;

public sealed class GetWorkspacesByUserHandler(
    ISqlRepository<Member> sqlRepository,
    IMapper mapper,
    ILogger logger,
    IUserIdGetter userIdGetter)
    : EfQueryPaginationHandler<Member, GetWorkspacesByUserQuery, WorkspaceResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryListFlowBuilder<Member, WorkspaceResponse> BuildQueryFlow(
        IQueryListFilter<Member, WorkspaceResponse> fromFlow, GetWorkspacesByUserQuery query)
        => fromFlow
            .WithFilter(x => x.UserId == userIdGetter.UserId)
            .WithSpecialAction(x =>
                x.Select(a => a.Workspace).ProjectTo<WorkspaceResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(x => x.Workspace.CreatedTime)
            .WithSortedDirectionWhenNotSet(SortedDirection.Descending);
}