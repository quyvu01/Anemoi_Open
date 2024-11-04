using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Workspace.ModelIds;

namespace Anemoi.Workspace.Domain.Models;

public sealed class MemberInvitation : ValueObject
{
    public MemberInvitationId Id { get; set; }
    public WorkspaceId WorkspaceId { get; set; }
    public Workspace Workspace { get; set; }
    public string Email { get; set; }
    public string CreatorId { get; set; }
    public List<string> RoleGroupIds { get; set; }
    public List<MemberInvitationMapOrganization> MemberInvitationMapOrganizations { get; set; }
    public DateTime CreatedTime { get; set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}