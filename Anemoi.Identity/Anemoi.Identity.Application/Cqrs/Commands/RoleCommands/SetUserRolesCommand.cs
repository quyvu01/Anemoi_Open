using System.Collections.Generic;
using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Identity.Domain.Models;
using OneOf;

namespace Anemoi.Identity.Application.Cqrs.Commands.RoleCommands;

public record SetUserRolesCommand(OneOf<User, UserId> UserIds, List<RoleId> RoleIds)
    : ICommand<OneOf<List<Role>, ErrorDetail>>;