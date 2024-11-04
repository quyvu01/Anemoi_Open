using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Workspace.ModelIds;

namespace Anemoi.Contract.Workspace.Commands.MemberMapRoleGroupCommands.UpdateMemberByUserId;

public sealed record UpdateMemberByUserIdCommand(string UserId, WorkspaceId WorkspaceId, List<string> RoleGroupIds)
    : ICommandVoid;