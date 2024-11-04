using System.Linq;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Infrastructure.Helpers;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Contract.Identity.Queries.RoleGroupQueries.GetCrossCuttingRoleGroups;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Queries.RoleGroupQueries.GetCrossCuttingRoleGroups;

public sealed class GetCrossCuttingRoleGroupsHandler(
    ISqlRepository<RoleGroup> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryCrossCuttingHandler<RoleGroup, GetCrossCuttingRoleGroupsQuery>(sqlRepository, mapper, logger,
        x => d => x.SelectorIds.Select(a => new RoleGroupId(a)).Contains(d.Id),
        x => new CrossCuttingDataResponse { Id = x.Id.ToString(), Value = x.Name });