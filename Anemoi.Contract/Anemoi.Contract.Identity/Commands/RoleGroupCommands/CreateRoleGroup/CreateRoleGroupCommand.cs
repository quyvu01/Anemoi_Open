using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.Contracts;
using Anemoi.Contract.Identity.ModelIds;
using Newtonsoft.Json;

namespace Anemoi.Contract.Identity.Commands.RoleGroupCommands.CreateRoleGroup;

public sealed record CreateRoleGroupCommand(
    string Name,
    string Description,
    List<RoleGroupClaimContract> RoleGroupClaims,
    List<RoleId> IdentityRoleIds,
    [property: JsonIgnore] bool IsDefault) : ICommandVoid;