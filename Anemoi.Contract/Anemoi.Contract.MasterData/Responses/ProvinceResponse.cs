using Anemoi.BuildingBlock.Application.Responses;

namespace Anemoi.Contract.MasterData.Responses;

public sealed class ProvinceResponse : ModelResponse
{
    public string Name { get; set; }
    public string Slug { get; set; }
}