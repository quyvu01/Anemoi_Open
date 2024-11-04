using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Workspace.ModelIds;

namespace Anemoi.Contract.Workspace.Commands.OrganizationCommands.CreateOrganization;

public sealed record CreateOrganizationCommand : ICommandVoid
{
    public OrganizationId ParentOrganizationId { get; set; }
    public string LogoPath { get; set; }
    public string CoverPhotoPath { get; set; }
    public string Name { get; set; }
    public string Website { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string ProvinceId { get; set; }
    public string DistrictId { get; set; }
    public string Address { get; set; }
    public string TaxCode { get; set; }
    public string Owner { get; set; }
    public string SubDomain { get; set; }
}