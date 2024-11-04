using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Contract.Identity.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotChocolate;
using HotChocolate.Data;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.GraphQl.Queries;

public sealed class IdentityQueries
{
    [UseProjection]
    public IQueryable<UserResponse> GetUsers(
        [Service] ISqlRepository<User> sqlRepository,
        [Service] IMapper mapper, List<Guid> ids)
    {
        Expression<Func<User, bool>> idsFilter = ids switch
        {
            { } val => p => val.Select(a => new UserId(a)).Contains(p.UserId),
            _ => _ => true
        };
        return sqlRepository
            .GetQueryable(idsFilter)
            .ProjectTo<UserResponse>(mapper.ConfigurationProvider);
    }
}