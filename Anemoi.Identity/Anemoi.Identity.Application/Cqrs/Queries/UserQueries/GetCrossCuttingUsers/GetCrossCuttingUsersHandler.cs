using System.Linq;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Infrastructure.Helpers;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Contract.Identity.Queries.UserQueries.GetCrossCuttingUsers;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Queries.UserQueries.GetCrossCuttingUsers;

public sealed class GetCrossCuttingUsersHandler(
    ISqlRepository<User> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryCrossCuttingHandler<User, GetCrossCuttingUsersQuery>(sqlRepository, mapper, logger,
        x => d => x.SelectorIds.Select(a => new UserId(a)).Contains(d.UserId),
        x => new CrossCuttingDataResponse { Id = x.UserId.ToString(), Value = $"{x.FirstName} {x.LastName}".Trim() });