using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.RoleGroupCommands.RemoveRoleGroup;
using Anemoi.Contract.Identity.Errors;
using AutoMapper;
using Serilog;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.Cqrs.Commands.RoleGroupCommands.RemoveRoleGroup;

public sealed class RemoveRoleGroupHandler(
    ISqlRepository<RoleGroup> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger,
    ISqlRepository<UserMapRoleGroup> userRoleGroupDbRepository)
    : EfCommandOneVoidHandler<RoleGroup, RemoveRoleGroupCommand>(sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<RoleGroup> BuildCommand(IStartOneCommandVoid<RoleGroup> fromFlow,
        RemoveRoleGroupCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .RemoveOne(a => a.Id == command.Id)
            .WithSpecialAction(null)
            .WithCondition(async _ =>
            {
                var isApplied = await userRoleGroupDbRepository
                    .ExistByConditionAsync(a => a.RoleGroupId == command.Id, cancellationToken);
                if (isApplied) return IdentityErrorDetail.RoleGroupError.Applied();

                var checkDefault = await SqlRepository.ExistByConditionAsync(
                    x => x.Id == command.Id && x.IsDefault, cancellationToken);
                if (checkDefault) return IdentityErrorDetail.RoleGroupError.RoleGroupDefault();

                return None.Value;
            })
            .WithErrorIfNull(IdentityErrorDetail.RoleGroupError.NotFound())
            .WithErrorIfSaveChange(IdentityErrorDetail.RoleGroupError.RemoveFailed());
}