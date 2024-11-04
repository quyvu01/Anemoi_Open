using System.Linq;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Identity.Queries.IdentityPolicyQueries.GetIdentityPolicies;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Queries.IdentityPolicyQueries.GetIdentityPolicies;

public sealed class GetIdentityPoliciesHandler(
    ISqlRepository<IdentityPolicy> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryCollectionHandler<IdentityPolicy, GetIdentityPoliciesQuery, IdentityPolicyResponse>(sqlRepository, mapper,
        logger)
{
    protected override IQueryListFlowBuilder<IdentityPolicy, IdentityPolicyResponse> BuildQueryFlow(
        IQueryListFilter<IdentityPolicy, IdentityPolicyResponse> fromFlow, GetIdentityPoliciesQuery query)
        => fromFlow
            .WithFilter(null)
            .WithSpecialAction(x => x
                .Select(i => new IdentityPolicyResponse { Key = i.Key, Value = i.Value, Id = i.Id.ToString() }))
            .WithSortFieldWhenNotSet(a => a.Key)
            .WithSortedDirectionWhenNotSet(SortedDirection.Ascending);
}