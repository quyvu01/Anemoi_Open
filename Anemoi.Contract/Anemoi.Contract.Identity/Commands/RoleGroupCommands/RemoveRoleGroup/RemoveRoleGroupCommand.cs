using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.ModelIds;

namespace Anemoi.Contract.Identity.Commands.RoleGroupCommands.RemoveRoleGroup;

public sealed record RemoveRoleGroupCommand(RoleGroupId Id) : ICommandVoid;