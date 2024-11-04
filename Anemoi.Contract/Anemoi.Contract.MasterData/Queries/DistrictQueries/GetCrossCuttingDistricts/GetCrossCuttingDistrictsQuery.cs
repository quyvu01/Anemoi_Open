using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.Contract.CrossCuttingConcern.Attributes;

namespace Anemoi.Contract.MasterData.Queries.DistrictQueries.GetCrossCuttingDistricts;

public record GetCrossCuttingDistrictsQuery(List<Guid> SelectorIds, string Expression)
    : DataMappableOf<DistrictOfAttribute>(SelectorIds, Expression);