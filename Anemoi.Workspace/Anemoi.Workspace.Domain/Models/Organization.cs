using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Workspace.ModelIds;

namespace Anemoi.Workspace.Domain.Models;

public sealed class Organization : ValueObject
{
    public OrganizationId Id { get; set; }
    public WorkspaceId WorkspaceId { get; set; }
    public Workspace Workspace { get; set; }
    public OrganizationId ParentOrganizationId { get; set; }
    public Organization ParentOrganization { get; set; }
    public List<Organization> ChildOrganizations { get; set; }
    public List<MemberMapOrganization> MemberMapOrganizations { get; set; }
    public string LogoPath { get; set; }
    public string CoverPhotoPath { get; set; }
    public string Name { get; set; }
    public string SearchHint { get; set; }
    public string TaxCode { get; set; }
    public string ProvinceId { get; set; }
    public string DistrictId { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Owner { get; set; }
    public string Website { get; set; }
    public DateTime CreatedTime { get; set; }
    public bool IsRemoved { get; set; }
    public string SubDomain { get; set; }
    public string CustomDomain { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}