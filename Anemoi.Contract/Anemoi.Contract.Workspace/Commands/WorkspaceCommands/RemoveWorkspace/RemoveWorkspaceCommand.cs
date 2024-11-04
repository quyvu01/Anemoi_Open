using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Workspace.ModelIds;

namespace Anemoi.Contract.Workspace.Commands.WorkspaceCommands.RemoveWorkspace;

public sealed record RemoveWorkspaceCommand(WorkspaceId WorkspaceId) : ICommandVoid;