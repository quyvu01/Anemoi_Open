using Anemoi.BuildingBlock.Application.Responses;

namespace Anemoi.BuildingBlock.Application.Errors;

public static class GlobalErrors
{
    public static ErrorDetailResponse UnexpectedError() => new()
    {
        Messages = ["UnexpectedError"], Code = "500"
    };
}