using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Identity.Queries.RoleQueries.GetRoles;
using Anemoi.Contract.Identity.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.Cqrs.Queries.RoleQueries.GetRoles;

public sealed class UserRolesHandler(
    ISqlRepository<Role> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryPaginationHandler<Role, GetUserRolesQuery, UserRoleResponse>(
        sqlRepository, mapper, logger)
{
    protected override IQueryListFlowBuilder<Role, UserRoleResponse> BuildQueryFlow(
        IQueryListFilter<Role, UserRoleResponse> fromFlow,
        GetUserRolesQuery query) => fromFlow
        .WithFilter(null)
        .WithSpecialAction(x => x
            .ProjectTo<UserRoleResponse>(Mapper.ConfigurationProvider))
        .WithSortFieldWhenNotSet(r => r.Name)
        .WithSortedDirectionWhenNotSet(SortedDirection.Ascending);
}