using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Workspace.ModelIds;

namespace Anemoi.Workspace.Domain.Models;

public sealed class MemberInvitationMapOrganization : ValueObject
{
    public MemberInvitationMapOrganizationId Id { get; set; }
    public MemberInvitationId MemberInvitationId { get; set; }
    public MemberInvitation MemberInvitation { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public Organization Organization { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}