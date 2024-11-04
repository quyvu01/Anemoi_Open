using Anemoi.BuildingBlock.Application.Responses;

namespace Anemoi.Contract.Identity.Responses;

public sealed class RoleGroupResponse : ModelResponse
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedTime { get; set; }
    public bool IsActivated { get; set; }
    public bool IsDefault { get; set; }
    public List<RoleGroupClaimResponse> RoleGroupClaims { get; set; }
    public List<UserRoleResponse> IdentityRoles { get; set; }
}