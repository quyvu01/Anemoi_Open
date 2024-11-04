using System;
using System.Linq;
using System.Linq.Expressions;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Identity.Contracts;
using Anemoi.Contract.Identity.Queries.RoleQueries.GetUserRolesByRoleGroupClaims;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Queries.RoleQueries.GetUserRolesByWorkspace;

public sealed class GetUserRolesByWorkspaceHandler(
    ISqlRepository<UserMapRoleGroup> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryCollectionHandler<UserMapRoleGroup, GetUserRolesByRoleGroupClaimsQuery, RoleGroupResponse>(sqlRepository,
        mapper,
        logger)
{
    protected override IQueryListFlowBuilder<UserMapRoleGroup, RoleGroupResponse> BuildQueryFlow(
        IQueryListFilter<UserMapRoleGroup, RoleGroupResponse> fromFlow,
        GetUserRolesByRoleGroupClaimsQuery query)
    {
        var roleGroupClaimsFilter = query.RoleGroupClaims switch
        {
            { Count: > 0 } claims => claims
                .Select<RoleGroupClaimContract, Expression<Func<UserMapRoleGroup, bool>>>(k =>
                    r => r.RoleGroup.RoleGroupClaims.Any(x => x.Key == k.Key && x.Value == k.Value))
                .Aggregate(ExpressionHelper.OrElse),
            _ => _ => true
        };
        return fromFlow
            .WithFilter(roleGroupClaimsFilter.And(a => a.UserId == query.UserId))
            .WithSpecialAction(a => a
                .Select(x => x.RoleGroup)
                .ProjectTo<RoleGroupResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(a => a.Id)
            .WithSortedDirectionWhenNotSet(SortedDirection.Ascending);
    }
}