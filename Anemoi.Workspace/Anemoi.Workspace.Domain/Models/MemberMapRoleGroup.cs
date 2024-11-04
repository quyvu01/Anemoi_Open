using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Workspace.ModelIds;

namespace Anemoi.Workspace.Domain.Models;

public sealed class MemberMapRoleGroup : ValueObject
{
    public MemberMapRoleGroupId Id { get; set; }
    public MemberId MemberId { get; set; }
    public Member Member { get; set; }
    public string RoleGroupId { get; set; }
    public int Order { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}