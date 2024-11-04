using System;
using System.Linq;
using System.Linq.Expressions;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Contract.Identity.Queries.RoleGroupQueries.GetRoleGroups;
using Anemoi.Contract.Identity.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.Cqrs.Queries.RoleGroupQueries.GetRoleGroups;

public sealed class GetRoleGroupsHandler(ISqlRepository<RoleGroup> sqlRepository, IMapper mapper, ILogger logger)
    : EfQueryPaginationHandler<RoleGroup, GetRoleGroupsQuery, RoleGroupsResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryListFlowBuilder<RoleGroup, RoleGroupsResponse> BuildQueryFlow(
        IQueryListFilter<RoleGroup, RoleGroupsResponse> fromFlow, GetRoleGroupsQuery query)
    {
        Expression<Func<RoleGroup, bool>> searchKeyFilter = query.SearchKey switch
        {
            { } val => r => r.SearchHint.Contains(val.GenerateSearchHint()),
            _ => _ => true
        };
        Expression<Func<RoleGroup, bool>> defaultFilter = query.IsDefault switch
        {
            { } val => r => r.IsDefault == val,
            _ => _ => true
        };

        Expression<Func<RoleGroup, bool>> creatorIdsFilter = query.CreatorIds switch
        {
            { } val => r => val.Select(x => new UserId(Guid.Parse(x))).Contains(r.CreatorId),
            _ => _ => true
        };

        var roleGroupClaimsFilter = query.RoleGroupClaimKeys switch
        {
            { Count: > 0 } keys => keys
                .Select<string, Expression<Func<RoleGroup, bool>>>(k => r => r.RoleGroupClaims.Any(x => x.Key == k))
                .Aggregate(ExpressionHelper.OrElse),
            _ => _ => true
        };

        var finalFilter = ExpressionHelper
            .CombineAnd(searchKeyFilter, defaultFilter, creatorIdsFilter, roleGroupClaimsFilter);
        return fromFlow
            .WithFilter(finalFilter)
            .WithSpecialAction(r => r.ProjectTo<RoleGroupsResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(x => x.CreatedTime)
            .WithSortedDirectionWhenNotSet(SortedDirection.Descending);
    }
}