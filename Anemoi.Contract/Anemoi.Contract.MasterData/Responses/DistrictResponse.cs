using Anemoi.BuildingBlock.Application.Responses;

namespace Anemoi.Contract.MasterData.Responses;

public sealed class DistrictResponse : ModelResponse
{
    public string ProvinceId { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
}