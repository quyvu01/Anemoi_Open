using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Infrastructure.Helpers;
using Anemoi.Contract.MasterData.ModelIds;
using Anemoi.Contract.MasterData.Queries.DistrictQueries.GetCrossCuttingDistricts;
using Anemoi.MasterData.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.MasterData.Application.Cqrs.Queries.DistrictQueries.GetCrossCuttingDistricts;

public sealed class GetCrossCuttingDistrictsHandler(
    ISqlRepository<District> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryCrossCuttingHandler<District, GetCrossCuttingDistrictsQuery>(sqlRepository, mapper, logger,
        x => d => x.SelectorIds.Select(a => new DistrictId(a)).Contains(d.Id),
        x => new CrossCuttingDataResponse { Id = x.Id.ToString(), Value = x.Name });