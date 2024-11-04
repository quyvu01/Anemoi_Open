using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.Contract.MasterData.ModelIds;
using Anemoi.Contract.MasterData.Responses;

namespace Anemoi.Contract.MasterData.Queries.DistrictQueries.GetDistricts;

public sealed record GetDistrictsQuery(ProvinceId ProvinceId, string SearchKey) : GetManyQuery,
    IQueryPaged<DistrictResponse>;