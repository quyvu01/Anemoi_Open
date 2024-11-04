using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryOne;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.Queries.IdentityQueries.CheckUserExist;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Queries.IdentityQueries.CheckUserExist;

public sealed class CheckUserExistHandler(
    ISqlRepository<User> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryOneHandler<User,
        CheckUserExistQuery, UserWithEmailResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryOneFlowBuilder<User, UserWithEmailResponse> BuildQueryFlow(
        IQueryOneFilter<User, UserWithEmailResponse> fromFlow,
        CheckUserExistQuery query)
        => fromFlow
            .WithFilter(x => x.Email == query.Email && x.IsActivated)
            .WithSpecialAction(x => x.ProjectTo<UserWithEmailResponse>(Mapper.ConfigurationProvider))
            .WithErrorIfNull(IdentityErrorDetail.UserError.NotFound());
}