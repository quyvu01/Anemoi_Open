using Anemoi.BuildingBlock.Application.Responses;

namespace Anemoi.Contract.Identity.Responses;

public sealed class RoleGroupsResponse : ModelResponse
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedTime { get; set; }
    public string CreatorId { get; set; }
    public string CreatorName { get; set; }
    public string CreatorEmail { get; set; }
    public string UpdaterId { get; set; }
    public string UpdaterName { get; set; }
    public string UpdaterEmail { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public bool IsDefault { get; set; }
}