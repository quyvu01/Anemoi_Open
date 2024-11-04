using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryOne;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.Queries.UserQueries.GetUser;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Queries.UserQueries.GetUser;

public sealed class GetUserHandler(
    ISqlRepository<User> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryOneHandler<User, GetUserQuery, UserResponse>(sqlRepository, mapper,
        logger)
{
    protected override IQueryOneFlowBuilder<User, UserResponse> BuildQueryFlow(
        IQueryOneFilter<User, UserResponse> fromFlow, GetUserQuery query)
        => fromFlow
            .WithFilter(x => x.UserId == query.Id)
            .WithSpecialAction(x => x)
            .WithErrorIfNull(IdentityErrorDetail.UserError.NotFound());
}