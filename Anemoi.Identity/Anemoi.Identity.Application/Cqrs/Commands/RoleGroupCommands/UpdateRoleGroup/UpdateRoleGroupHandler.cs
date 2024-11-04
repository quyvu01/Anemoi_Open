using System.Linq;
using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.RoleGroupCommands.UpdateRoleGroup;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.ModelIds;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.Cqrs.Commands.RoleGroupCommands.UpdateRoleGroup;

public sealed class UpdateRoleGroupHandler(
    ISqlRepository<RoleGroup> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger,
    ISqlRepository<RoleGroupMapRole> dbRoleGroupIdentityRoleRepository)
    : EfCommandOneVoidHandler<RoleGroup, UpdateRoleGroupCommand>(sqlRepository,
        unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<RoleGroup> BuildCommand(IStartOneCommandVoid<RoleGroup> fromFlow,
        UpdateRoleGroupCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .UpdateOne(x => x.Id == command.Id)
            .WithSpecialAction(r => r
                .Include(x => x.RoleGroupMapRoles))
            .WithCondition(async roleGroup =>
            {
                var checkDefault = await SqlRepository.ExistByConditionAsync(
                    x => x.Id == command.Id && x.IsDefault, cancellationToken);
                if (checkDefault) return IdentityErrorDetail.RoleGroupError.RoleGroupDefault();

                Mapper.Map(command, roleGroup);
                var roleIds = command.IdentityRoleIds;
                if (!roleIds.HasAny()) return None.Value;
                var roleGroupIdentityRoleIds = roleGroup.RoleGroupMapRoles
                    .Select(x => x.RoleId).ToList();
                var idRoleGroupIdentityRolesRemoving = roleGroupIdentityRoleIds.Except(roleIds).ToList();
                var idsAdding = roleIds.Except(roleGroupIdentityRoleIds);
                var idsRemoving = roleGroup.RoleGroupMapRoles.Where(x =>
                    idRoleGroupIdentityRolesRemoving.Contains(x.RoleId)).ToList();
                await dbRoleGroupIdentityRoleRepository.RemoveManyAsync(idsRemoving, cancellationToken);
                var roleGroupIdentityRoles = idsAdding.Select(roleId =>
                    new RoleGroupMapRole
                    {
                        Id = new RoleGroupMapUserRoleId(IdGenerator.NextGuid()),
                        RoleGroupId = roleGroup.Id,
                        RoleId = roleId
                    }).ToList();
                await dbRoleGroupIdentityRoleRepository.CreateManyAsync(roleGroupIdentityRoles, cancellationToken);
                return None.Value;
            })
            .WithModify(roleGroup => Mapper.Map(command, roleGroup))
            .WithErrorIfNull(IdentityErrorDetail.RoleGroupError.NotFound())
            .WithErrorIfSaveChange(IdentityErrorDetail.RoleGroupError.UpdateFailed());
}