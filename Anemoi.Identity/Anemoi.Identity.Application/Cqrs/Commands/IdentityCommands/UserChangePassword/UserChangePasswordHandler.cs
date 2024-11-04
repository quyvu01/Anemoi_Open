using System;
using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.IdentityCommands.UserChangePassword;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.UserChangePassword;

public sealed class UserChangePasswordHandler(
    IMapper mapper,
    ILogger logger,
    IUserRepository userRepository,
    ISqlRepository<User> userDbRepository,
    IUnitOfWork unitOfWork,
    IUserIdGetter userIdGetter)
    : EfCommandOneResultHandler<User, UserChangePasswordCommand, UserIdResponse>(
        userDbRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderResult<User, UserIdResponse> BuildCommand(
        IStartOneCommandResult<User, UserIdResponse> fromFlow,
        UserChangePasswordCommand command, CancellationToken cancellationToken) =>
        fromFlow
            .UpdateOne(x => x.UserId == new UserId(Guid.Parse(userIdGetter.UserId)) && x.IsActivated)
            .WithSpecialAction(null)
            .WithCondition(async user =>
            {
                var changePasswordResult = await userRepository
                    .ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);
                user.ChangedPasswordTime = DateTime.UtcNow;
                return changePasswordResult.MapT1(_ =>
                    IdentityErrorDetail.IdentityError.FailedToChangePassword());
            })
            .WithModify(_ => { })
            .WithErrorIfNull(IdentityErrorDetail.UserError.NotFound())
            .WithErrorIfSaveChange(IdentityErrorDetail.IdentityError.FailedToChangePassword())
            .WithResultIfSucceed(user => new UserIdResponse { Id = user.UserId.ToString() });
}