using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.Contract.CrossCuttingConcern.Attributes;

namespace Anemoi.Contract.MasterData.Queries.ProvinceQueries.GetCrossCuttingProvinces;

public record GetCrossCuttingProvincesQuery(List<Guid> SelectorIds, string Expression)
    : DataMappableOf<ProvinceOfAttribute>(SelectorIds, Expression);