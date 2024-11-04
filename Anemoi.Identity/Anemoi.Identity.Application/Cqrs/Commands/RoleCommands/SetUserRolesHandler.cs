using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Identity.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Anemoi.Identity.Domain.Models;
using OneOf;

namespace Anemoi.Identity.Application.Cqrs.Commands.RoleCommands;

public sealed class SetUserRolesHandler(
    IUserRepository userRepository,
    ISqlRepository<Role> userRoleRepository,
    ISqlRepository<User> userDbRepository)
    : ICommandHandler<SetUserRolesCommand, OneOf<List<Role>, ErrorDetail>>
{
    public async Task<OneOf<List<Role>, ErrorDetail>> Handle(SetUserRolesCommand request,
        CancellationToken cancellationToken)
    {
        var (userOrId, roleIds) = request;
        if (roleIds is null) return IdentityErrorDetail.RoleError.RolesRequestMustNotBeNull();
        if (roleIds.Distinct().Count() != roleIds.Count) return IdentityErrorDetail.RoleError.RolesRequestDuplicated();
        var user = await userOrId.Match(Task.FromResult, id => userDbRepository
            .GetFirstByConditionAsync(x => x.UserId == id && x.IsActivated, null, cancellationToken));
        if (user is null) return IdentityErrorDetail.UserError.NotFound();
        var roles = await userRoleRepository
            .GetManyByConditionAsync(r => roleIds.Contains(r.RoleId), db => db.AsNoTracking(),
                token: cancellationToken);
        if (roles.Count != roleIds.Count) return IdentityErrorDetail.RoleError.RolesNotExist();
        var removeRolesResult = await RemoveRolesFromUserAsync(user);
        if (removeRolesResult.IsT1) return removeRolesResult.AsT1;
        if (roleIds is { Count: 0 }) return roles;
        var addRolesResult = await userRepository.AddToRolesAsync(user, roles.Select(r => r.Name));
        return addRolesResult.MapT0(_ => roles)
            .MapT1(_ => IdentityErrorDetail.RoleError.AddRolesError());
    }

    private async Task<OneOf<None, ErrorDetail>> RemoveRolesFromUserAsync(User user)
    {
        var existRoles = await userRepository.GetRolesAsync(user);
        var removeRolesResult = await userRepository.RemoveFromRolesAsync(user, existRoles);
        return removeRolesResult.MapT1(_ => IdentityErrorDetail.RoleError.RemoveRolesError());
    }
}