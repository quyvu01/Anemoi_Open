using Anemoi.BuildingBlock.Application.Abstractions;

namespace Anemoi.BuildingBlock.Infrastructure.Services;

public sealed class WorkspaceService : IWorkspaceIdSetter, IWorkspaceIdGetter
{
    public void SetWorkspaceId(string workspaceId) => WorkspaceId = workspaceId;
    public string WorkspaceId { get; private set; }
}