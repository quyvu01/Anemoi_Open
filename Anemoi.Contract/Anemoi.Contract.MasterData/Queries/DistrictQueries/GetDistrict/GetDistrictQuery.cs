using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.MasterData.ModelIds;
using Anemoi.Contract.MasterData.Responses;

namespace Anemoi.Contract.MasterData.Queries.DistrictQueries.GetDistrict;

public sealed record GetDistrictQuery(DistrictId Id) : IQueryOne<DistrictResponse>;