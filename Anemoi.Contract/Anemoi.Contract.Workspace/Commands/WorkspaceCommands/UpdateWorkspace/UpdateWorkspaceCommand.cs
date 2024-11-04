using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Workspace.ModelIds;
using Newtonsoft.Json;

namespace Anemoi.Contract.Workspace.Commands.WorkspaceCommands.UpdateWorkspace;

public sealed record UpdateWorkspaceCommand([property: JsonIgnore] WorkspaceId Id, string LogoPath, string Name)
    : ICommandVoid;