using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.Contract.MasterData.Responses;

namespace Anemoi.Contract.MasterData.Queries.ProvinceQueries.GetProvinces;

public sealed record GetProvincesQuery(string SearchKey) : GetManyQuery, IQueryPaged<ProvinceResponse>;