using System.Collections.Generic;
using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.ModelIds;

namespace Anemoi.Identity.Application.Cqrs.Commands.UserMapRoleGroupCommands.UpdateUserMapRoleGroups;

public sealed record UpdateUserMapRoleGroupsCommand(UserId UserId, List<RoleGroupId> RoleGroupIds) : ICommandVoid;