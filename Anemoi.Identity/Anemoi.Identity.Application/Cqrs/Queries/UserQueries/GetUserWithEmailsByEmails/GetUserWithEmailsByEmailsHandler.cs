using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Identity.Queries.UserQueries.GetUserWithEmailsByEmails;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Queries.UserQueries.GetUserWithEmailsByEmails;

public sealed class GetUserWithEmailsByEmailsHandler(
    ISqlRepository<User> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryCollectionHandler<User, GetUserWithEmailsByEmailsQuery, UserResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryListFlowBuilder<User, UserResponse> BuildQueryFlow(
        IQueryListFilter<User, UserResponse> fromFlow, GetUserWithEmailsByEmailsQuery query)
        => fromFlow
            .WithFilter(x => query.Emails.Contains(x.Email))
            .WithSpecialAction(x => x.ProjectTo<UserResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(x => x.CreatedTime)
            .WithSortedDirectionWhenNotSet(SortedDirection.Descending);
}