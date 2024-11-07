using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.CrossCuttingConcern.Attributes;
using Anemoi.Contract.Workspace.ValueTypes;

namespace Anemoi.Contract.Workspace.Responses;

public sealed class WorkspaceResponse : ModelResponse
{
    public string LogoPath { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public string Name { get; set; }
    public DateTime CreatedTime { get; set; }
    public WorkspaceStateResponse State { get; set; }
    public List<MemberResponse> Members { get; set; }
}