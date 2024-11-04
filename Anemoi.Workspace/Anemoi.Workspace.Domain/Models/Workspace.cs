using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Workspace.Domain.Models.ValueTypes;

namespace Anemoi.Workspace.Domain.Models;

public sealed class Workspace : ValueObject
{
    public WorkspaceId Id { get; set; }
    public string LogoPath { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string SearchHint { get; set; }
    public bool IsRemoved { get; set; }
    public DateTime CreatedTime { get; set; }
    public List<Organization> Organizations { get; set; }
    public List<MemberInvitation> MemberInvitations { get; set; }
    public List<Member> Members { get; set; }
    public WorkspaceState State { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}