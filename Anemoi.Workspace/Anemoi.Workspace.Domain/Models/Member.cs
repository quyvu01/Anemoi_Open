using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Workspace.ModelIds;

namespace Anemoi.Workspace.Domain.Models;

public sealed class Member : ValueObject
{
    public MemberId Id { get; set; }
    public WorkspaceId WorkspaceId { get; set; }
    public Workspace Workspace { get; set; }
    public List<MemberMapOrganization> MemberMapOrganizations { get; set; }
    public string UserId { get; set; }
    public bool IsActivated { get; set; }
    public DateTime CreatedTime { get; set; }
    public List<MemberMapRoleGroup> MemberMapRoleGroups { get; set; }
    public bool IsRemoved { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}