using Anemoi.BuildingBlock.Application.Responses;

namespace Anemoi.Contract.Identity.Responses;

public sealed class UserRoleResponse : ModelResponse
{
    public string Name { get; set; }
}