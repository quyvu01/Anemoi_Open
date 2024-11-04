using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Workspace.ModelIds;

namespace Anemoi.Workspace.Domain.Models;

public class MemberMapOrganization : ValueObject
{
    public MemberMapOrganizationId Id { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public Organization Organization { get; set; }
    public MemberId MemberId { get; set; }
    public Member Member { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}