using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.CrossCuttingConcern.Attributes;

namespace Anemoi.Contract.Workspace.Responses;

public sealed class MemberInvitationResponse : ModelResponse
{
    public string WorkspaceId { get; set; }
    public string Email { get; set; }
    public DateTime CreatedTime { get; set; }
    public string CreatorId { get; set; }
    [UserOf(nameof(CreatorId))] public string CreatorName { get; set; }

    [UserOf(nameof(CreatorId), Expression = "Email")]
    public string CreatorEmail { get; set; }

    public string RoleGroupId { get; set; }
    public int MemberInvitationStatus { get; set; }
}