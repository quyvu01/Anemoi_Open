using Anemoi.BuildingBlock.Application.Responses;

namespace Anemoi.Contract.Identity.Responses;

public class RoleGroupClaimResponse : ModelResponse
{
    public string Key { get; set; }
    public string Value { get; set; }
}