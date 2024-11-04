using System.Linq;
using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandMany;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Commands.UserMapRoleGroupCommands.UpdateUserMapRoleGroups;

public sealed class UpdateUserMapRoleGroupsHandler(
    ISqlRepository<UserMapRoleGroup> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandManyVoidHandler<UserMapRoleGroup, UpdateUserMapRoleGroupsCommand>(sqlRepository, unitOfWork, mapper,
        logger)
{
    protected override ICommandManyFlowBuilderVoid<UserMapRoleGroup> BuildCommand(
        IStartManyCommandVoid<UserMapRoleGroup> fromFlow, UpdateUserMapRoleGroupsCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .CreateMany(async () =>
            {
                var existRoleGroups = await SqlRepository
                    .GetQueryable(a => a.UserId == command.UserId && command.RoleGroupIds.Contains(a.RoleGroupId))
                    .ToListAsync(cancellationToken);
                await SqlRepository.RemoveManyAsync(existRoleGroups, cancellationToken);
                return command.RoleGroupIds.Select(roleGroupId => new UserMapRoleGroup
                {
                    Id = new UserMapRoleGroupId(IdGenerator.NextGuid()), UserId = command.UserId,
                    RoleGroupId = roleGroupId
                }).ToList();
            })
            .WithCondition(_ => None.Value)
            .WithErrorIfSaveChange(IdentityErrorDetail.UserMapRoleGroupError.CreateFailed());
}