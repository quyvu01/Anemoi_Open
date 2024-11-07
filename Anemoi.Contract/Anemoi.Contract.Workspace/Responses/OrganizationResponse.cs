using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.CrossCuttingConcern.Attributes;

namespace Anemoi.Contract.Workspace.Responses;

public sealed class OrganizationResponse : ModelResponse
{
    public string ParentOrganizationId { get; set; }
    public string LogoPath { get; set; }
    public string CoverPhotoPath { get; set; }
    public string Name { get; set; }
    public string TaxCode { get; set; }
    public string ProvinceId { get; set; }
    [ProvinceOf(nameof(ProvinceId))] public string ProvinceName { get; set; }
    public string DistrictId { get; set; }
    [DistrictOf(nameof(DistrictId))] public string DistrictName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Owner { get; set; }
    public string Website { get; set; }
    public string SubDomain { get; set; }
    public string CustomDomain { get; set; }
}