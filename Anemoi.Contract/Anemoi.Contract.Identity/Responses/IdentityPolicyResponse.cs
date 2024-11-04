using Anemoi.BuildingBlock.Application.Responses;

namespace Anemoi.Contract.Identity.Responses;

public class IdentityPolicyResponse : ModelResponse
{
    public string Key { get; set; }
    public string Value { get; set; }
    public List<UserRoleResponse> UserRoles { get; set; }
}