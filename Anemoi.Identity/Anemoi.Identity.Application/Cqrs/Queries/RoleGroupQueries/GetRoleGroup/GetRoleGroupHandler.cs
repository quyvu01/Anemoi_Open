using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryOne;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.Queries.RoleGroupQueries.GetRoleGroup;
using Anemoi.Contract.Identity.Responses;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.Cqrs.Queries.RoleGroupQueries.GetRoleGroup;

public sealed class GetRoleGroupHandler(ISqlRepository<RoleGroup> sqlRepository, IMapper mapper, ILogger logger)
    : EfQueryOneHandler<RoleGroup, GetRoleGroupQuery, RoleGroupResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryOneFlowBuilder<RoleGroup, RoleGroupResponse> BuildQueryFlow(
        IQueryOneFilter<RoleGroup, RoleGroupResponse> fromFlow, GetRoleGroupQuery query)
        => fromFlow
            .WithFilter(x => x.Id == query.RoleGroupId)
            .WithSpecialAction(x => x
                .Include(rg => rg.RoleGroupMapRoles)
                .ThenInclude(rg => rg.Role))
            .WithErrorIfNull(IdentityErrorDetail.RoleGroupError.NotFound());
}