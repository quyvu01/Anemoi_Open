using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryOne;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.Queries.IdentityPolicyQueries.GetIdentityPolicy;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Queries.IdentityPolicyQueries.GetIdentityPolicy;

public sealed class GetGetIdentityPolicyHandler(
    ISqlRepository<IdentityPolicy> sqlRepository,
    IApplicationPolicyGetter applicationPolicyGetter,
    IMapper mapper,
    ILogger logger)
    : EfQueryOneHandler<IdentityPolicy, GetIdentityPolicyQuery, IdentityPolicyResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryOneFlowBuilder<IdentityPolicy, IdentityPolicyResponse> BuildQueryFlow(
        IQueryOneFilter<IdentityPolicy, IdentityPolicyResponse> fromFlow, GetIdentityPolicyQuery query)
        => fromFlow
            .WithFilter(x => x.Key == applicationPolicyGetter.Key && x.Value == applicationPolicyGetter.Value)
            .WithSpecialAction(x => x.ProjectTo<IdentityPolicyResponse>(Mapper.ConfigurationProvider))
            .WithErrorIfNull(IdentityErrorDetail.ApplicationPolicyError.NotFound());
}