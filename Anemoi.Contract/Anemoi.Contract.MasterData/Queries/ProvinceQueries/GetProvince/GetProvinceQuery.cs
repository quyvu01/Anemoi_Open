using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.MasterData.ModelIds;
using Anemoi.Contract.MasterData.Responses;

namespace Anemoi.Contract.MasterData.Queries.ProvinceQueries.GetProvince;

public sealed record GetProvinceQuery(ProvinceId Id) : IQueryOne<ProvinceResponse>;