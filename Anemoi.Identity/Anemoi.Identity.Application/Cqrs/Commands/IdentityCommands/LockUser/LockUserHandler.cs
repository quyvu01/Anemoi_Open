using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.IdentityCommands.LockUser;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.LockUser;

public sealed class LockUserHandler(
    IMapper mapper,
    ILogger logger,
    IUserRepository userRepository,
    ISqlRepository<User> userDbRepository,
    IUnitOfWork unitOfWork)
    : EfCommandOneVoidHandler<User, LockUserCommand>(userDbRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<User> BuildCommand(
        IStartOneCommandVoid<User> fromFlow, LockUserCommand command,
        CancellationToken cancellationToken) => fromFlow
        .UpdateOne(x => x.UserId == command.UserId && x.IsActivated)
        .WithSpecialAction(null)
        .WithCondition(async user =>
        {
            if (!command.EnableLock) return None.Value;
            var lockResult = await userRepository.SetLockoutEnabledAsync(user, command.EnableLock);
            if (lockResult.IsT1)
                return IdentityErrorDetail.IdentityError.LockUserFailed();
            var lockEndDateResult = await userRepository
                .SetLockoutEndDateAsync(user, command.LockUntil);
            return lockEndDateResult.MapT1(_ => IdentityErrorDetail.IdentityError
                .LockUserFailed());
        })
        .WithModify(_ => { })
        .WithErrorIfNull(IdentityErrorDetail.UserError.NotFound())
        .WithErrorIfSaveChange(IdentityErrorDetail.IdentityError.LockUserFailed());
}