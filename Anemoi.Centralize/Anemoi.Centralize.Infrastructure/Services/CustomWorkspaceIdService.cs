using Anemoi.Centralize.Application.Abstractions;

namespace Anemoi.Centralize.Infrastructure.Services;

public class CustomWorkspaceIdService : ICustomWorkspaceIdGetter, ICustomWorkspaceIdSetter
{
    public string WorkspaceId { get; private set; }
    public void SetWorkspaceId(string workspaceId) => WorkspaceId = workspaceId;
}