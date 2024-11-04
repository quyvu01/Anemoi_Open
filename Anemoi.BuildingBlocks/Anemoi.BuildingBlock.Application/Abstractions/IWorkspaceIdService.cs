namespace Anemoi.BuildingBlock.Application.Abstractions;

public interface IWorkspaceIdGetter
{
    string WorkspaceId { get; }
}

public interface IWorkspaceIdSetter
{
    void SetWorkspaceId(string workspaceId);
}