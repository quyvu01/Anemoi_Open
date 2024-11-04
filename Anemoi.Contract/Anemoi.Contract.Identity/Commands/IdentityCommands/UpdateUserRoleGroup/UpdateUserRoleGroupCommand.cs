using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.ModelIds;
using Newtonsoft.Json;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.UpdateUserRoleGroup;

public sealed record UpdateUserRoleGroupCommand(RoleGroupId RoleGroupId, string WorkspaceId,
    [property: JsonIgnore] UserId UserId) : ICommandVoid;