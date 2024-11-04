using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.ModelIds;
using Newtonsoft.Json;

namespace Anemoi.Contract.Identity.Commands.RoleGroupCommands.UpdateRoleGroup;

public sealed record UpdateRoleGroupCommand([property: JsonIgnore] RoleGroupId Id,
    string Name, string Description, List<RoleId> IdentityRoleIds) : ICommandVoid;