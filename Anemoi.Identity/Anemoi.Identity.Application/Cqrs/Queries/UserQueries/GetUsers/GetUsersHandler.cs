using System;
using System.Linq.Expressions;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Identity.Queries.UserQueries.GetUsers;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Queries.UserQueries.GetUsers;

public sealed class GetUsersHandler(
    ISqlRepository<User> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryPaginationHandler<User, GetUsersQuery, UserResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryListFlowBuilder<User, UserResponse> BuildQueryFlow(
        IQueryListFilter<User, UserResponse> fromFlow, GetUsersQuery query) =>
        fromFlow
            .WithFilter(BuildFilter(query).And(x => x.IsActivated))
            .WithSpecialAction(a => a)
            .WithSortFieldWhenNotSet(x => x.CreatedTime)
            .WithSortedDirectionWhenNotSet(SortedDirection.Descending);

    private static Expression<Func<User, bool>> BuildFilter(GetUsersQuery query)
    {
        Expression<Func<User, bool>> searchFilterEmails = query.SearchKey switch
        {
            { } val => a => a.Email.Contains(val),
            _ => _ => true
        };

        var searchHint = query.SearchKey.GenerateSearchHint();
        Expression<Func<User, bool>> nameFilter = searchHint switch
        {
            { } val => a => a.SearchHint.Contains(val),
            _ => _ => true
        };

        Expression<Func<User, bool>> phoneNumber = query.SearchKey switch
        {
            { } val => a => a.PhoneNumber.Contains(val),
            _ => _ => true
        };

        return ExpressionHelper.CombineAnd(searchFilterEmails, nameFilter, phoneNumber);
    }
}