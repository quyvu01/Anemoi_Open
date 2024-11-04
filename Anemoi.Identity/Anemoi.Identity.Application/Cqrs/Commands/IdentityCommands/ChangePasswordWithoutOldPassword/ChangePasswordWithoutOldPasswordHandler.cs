using System;
using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.IdentityCommands.ChangePasswordWithoutOldPassword;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Application.Abstractions;
using AutoMapper;
using Serilog;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.ChangePasswordWithoutOldPassword;

public sealed class ChangePasswordWithoutOldPasswordHandler(
    IMapper mapper,
    ILogger logger,
    IUserRepository userRepository,
    ISqlRepository<User> userDbRepository,
    IUnitOfWork unitOfWork)
    : EfCommandOneResultHandler<User, ChangePasswordWithoutOldPasswordCommand, UserIdResponse>(
        userDbRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderResult<User, UserIdResponse> BuildCommand(
        IStartOneCommandResult<User, UserIdResponse> fromFlow,
        ChangePasswordWithoutOldPasswordCommand command, CancellationToken cancellationToken)
        => fromFlow
            .UpdateOne(x => x.UserId == command.UserId && x.IsActivated)
            .WithSpecialAction(null)
            .WithCondition(async user =>
            {
                var tokenPassword = await userRepository.GeneratePasswordResetTokenAsync(user);
                var resetPasswordResult = await userRepository
                    .ResetPasswordAsync(user, tokenPassword, command.NewPassword);
                return resetPasswordResult.MapT1(_ =>
                    IdentityErrorDetail.IdentityError.ResetUserPasswordFailed());
            })
            .WithModify(user => user.ChangedPasswordTime = DateTime.UtcNow)
            .WithErrorIfNull(IdentityErrorDetail.UserError.NotFound())
            .WithErrorIfSaveChange(
                IdentityErrorDetail.IdentityError.ResetUserPasswordFailed())
            .WithResultIfSucceed(user => new UserIdResponse { Id = user.UserId.ToString() });
}