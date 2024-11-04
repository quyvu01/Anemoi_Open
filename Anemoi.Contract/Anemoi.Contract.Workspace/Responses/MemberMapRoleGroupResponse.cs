using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.CrossCuttingConcern.Attributes;

namespace Anemoi.Contract.Workspace.Responses;

public sealed class MemberMapRoleGroupResponse : ModelResponse
{
    public string RoleGroupId { get; set; }
    [RoleGroupOf(nameof(RoleGroupId))] public string RoleGroupName { get; set; }
}