using System.Collections.Generic;

namespace Anemoi.BuildingBlock.Application.Responses;

public sealed class ErrorDetailResponse
{
    public IEnumerable<string> Messages { get; set; }
    public string Code { get; set; }
}