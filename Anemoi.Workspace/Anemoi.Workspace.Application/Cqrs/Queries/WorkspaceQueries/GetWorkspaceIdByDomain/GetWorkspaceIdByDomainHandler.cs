using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryOne;
using Anemoi.Contract.Workspace.Errors;
using Anemoi.Contract.Workspace.Queries.OrganizationQueries.GetOrganizationIdByDomain;
using Anemoi.Contract.Workspace.Responses;
using Anemoi.Workspace.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.Workspace.Application.Cqrs.Queries.WorkspaceQueries.GetWorkspaceIdByDomain;

public sealed class GetWorkspaceIdByDomainHandler(
    ISqlRepository<Organization> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryOneHandler<Organization, GetOrganizationIdByDomainQuery, OrganizationIdResponse>(
        sqlRepository, mapper, logger)
{
    protected override IQueryOneFlowBuilder<Organization, OrganizationIdResponse>
        BuildQueryFlow(IQueryOneFilter<Organization, OrganizationIdResponse> fromFlow,
            GetOrganizationIdByDomainQuery query)
        => fromFlow
            .WithFilter(a => a.SubDomain == query.Domain || a.CustomDomain == query.Domain)
            .WithSpecialAction(a => a.Select(x => new OrganizationIdResponse { Id = x.Id.ToString() }))
            .WithErrorIfNull(WorkspaceErrorDetail.WorkspaceError.NotFound());
}