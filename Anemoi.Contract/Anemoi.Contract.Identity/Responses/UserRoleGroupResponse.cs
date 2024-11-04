using Anemoi.BuildingBlock.Application.Responses;

namespace Anemoi.Contract.Identity.Responses;

public sealed class UserRoleGroupResponse : ModelResponse
{
    public string UserId { get; set; }
    public RoleGroupResponse RoleGroup { get; set; }
}