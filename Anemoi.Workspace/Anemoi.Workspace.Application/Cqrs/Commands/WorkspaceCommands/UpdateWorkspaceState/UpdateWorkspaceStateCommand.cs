using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Workspace.Domain.Models.ValueTypes;

namespace Anemoi.Workspace.Application.Cqrs.Commands.WorkspaceCommands.UpdateWorkspaceState;

public sealed record UpdateWorkspaceStateCommand(WorkspaceId WorkspaceId, WorkspaceState WorkspaceState) : ICommandVoid;