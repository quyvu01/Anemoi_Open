using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.CrossCuttingConcern.Attributes;

namespace Anemoi.Contract.Workspace.Responses;

public sealed class MemberResponse : ModelResponse
{
    public string UserId { get; set; }

    [UserOf(nameof(UserId), Expression = "Email")]
    public string UserEmail { get; set; }

    [UserOf(nameof(UserId))] public string UserName { get; set; }
    public DateTime CreatedTime { get; set; }
    public List<MemberMapRoleGroupResponse> MemberMapRoleGroups { get; set; }
    public bool IsActivated { get; set; }
    public bool IsRemoved { get; set; }
}