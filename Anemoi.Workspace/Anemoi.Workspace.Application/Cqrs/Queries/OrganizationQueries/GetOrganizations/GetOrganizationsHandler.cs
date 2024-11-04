using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Queries.OrganizationQueries.GetOrganizations;
using Anemoi.Contract.Workspace.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Cqrs.Queries.OrganizationQueries.GetOrganizations;

public sealed class GetOrganizationsHandler(
    ISqlRepository<Organization> sqlRepository,
    IMapper mapper,
    ILogger logger,
    IWorkspaceIdGetter workspaceIdGetter)
    : EfQueryPaginationHandler<Organization, GetOrganizationsQuery, OrganizationResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryListFlowBuilder<Organization, OrganizationResponse> BuildQueryFlow(
        IQueryListFilter<Organization, OrganizationResponse> fromFlow, GetOrganizationsQuery query)
        => fromFlow
            .WithFilter(x => x.WorkspaceId == new WorkspaceId(Guid.Parse(workspaceIdGetter.WorkspaceId)))
            .WithSpecialAction(x => x.ProjectTo<OrganizationResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(x => x.Id)
            .WithSortedDirectionWhenNotSet(SortedDirection.Ascending);
}