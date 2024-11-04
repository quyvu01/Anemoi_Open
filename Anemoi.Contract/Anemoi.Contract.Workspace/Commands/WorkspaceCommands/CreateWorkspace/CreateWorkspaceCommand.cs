using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Commands.WorkspaceCommands.CreateWorkspace;

public sealed record CreateWorkspaceCommand(string LogoPath, string SubDomain, string Name)
    : ICommandResult<WorkspaceIdResponse>;