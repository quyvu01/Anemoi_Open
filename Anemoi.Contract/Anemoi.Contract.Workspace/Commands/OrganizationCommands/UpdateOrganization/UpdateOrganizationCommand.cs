using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Workspace.ModelIds;
using Newtonsoft.Json;

namespace Anemoi.Contract.Workspace.Commands.OrganizationCommands.UpdateOrganization;

public sealed record UpdateOrganizationCommand([property: JsonIgnore] OrganizationId Id) : ICommandVoid
{
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
}