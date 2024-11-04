using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryOne;
using Anemoi.Contract.Workspace.Errors;
using Anemoi.Contract.Workspace.Queries.OrganizationQueries.GetOrganization;
using Anemoi.Contract.Workspace.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Cqrs.Queries.OrganizationQueries.GetOrganization;

public sealed class
    GetOrganizationHandler(ISqlRepository<Organization> sqlRepository, IMapper mapper, ILogger logger)
    : EfQueryOneHandler<Organization, GetOrganizationQuery, OrganizationResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryOneFlowBuilder<Organization, OrganizationResponse> BuildQueryFlow(
        IQueryOneFilter<Organization, OrganizationResponse> fromFlow, GetOrganizationQuery query)
        => fromFlow
            .WithFilter(x => x.Id == query.Id)
            .WithSpecialAction(x => x.ProjectTo<OrganizationResponse>(Mapper.ConfigurationProvider))
            .WithErrorIfNull(WorkspaceErrorDetail.OrganizationError.NotFound());
}