namespace Anemoi.Centralize.Application.Abstractions;

public interface ICustomWorkspaceIdSetter
{
    void SetWorkspaceId(string workspaceId);
}

public interface ICustomWorkspaceIdGetter
{
    string WorkspaceId { get; }
}