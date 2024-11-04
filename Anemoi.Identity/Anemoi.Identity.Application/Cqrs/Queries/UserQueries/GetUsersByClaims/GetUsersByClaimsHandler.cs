using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Identity.Queries.UserQueries.GetUsersByClaims;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Queries.UserQueries.GetUsersByClaims;

public sealed class GetUsersByClaimsHandler(
    ISqlRepository<User> sqlRepository,
    IMapper mapper,
    ILogger logger,
    IUserClaimRepository userClaimRepository)
    : EfQueryPaginationHandler<User, GetUsersByClaimsQuery, UserResponse>(sqlRepository, mapper, logger)
{
    public override async Task<PaginationResponse<UserResponse>> Handle(
        GetUsersByClaimsQuery request, CancellationToken cancellationToken)
    {
        var userIds = await userClaimRepository
            .GetUserIdsByClaimTypes(request.ClaimTypes);
        return await base.Handle(request with { UserIds = userIds }, cancellationToken);
    }

    protected override IQueryListFlowBuilder<User, UserResponse> BuildQueryFlow(
        IQueryListFilter<User, UserResponse> fromFlow,
        GetUsersByClaimsQuery query)
        => fromFlow
            .WithFilter(a => query.UserIds.Contains(a.Id))
            .WithSpecialAction(a => a.ProjectTo<UserResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(a => a.FirstName)
            .WithSortedDirectionWhenNotSet(SortedDirection.Ascending);
}