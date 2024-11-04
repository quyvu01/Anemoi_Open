using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.IdentityCommands.ChangePasswordWithoutOldPassword;
using Anemoi.Contract.Identity.Commands.IdentityCommands.ChangePasswordWithToken;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Application.Abstractions;
using AutoMapper;
using MediatR;
using Serilog;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.ChangePasswordWithToken;

public sealed class ChangePasswordWithTokenHandler(
    ISqlRepository<User> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger,
    IUserRepository userRepository,
    ISender sender)
    : EfCommandOneResultHandler<User, ChangePasswordWithTokenCommand, UserIdResponse>(
        sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderResult<User, UserIdResponse> BuildCommand(
        IStartOneCommandResult<User, UserIdResponse> fromFlow,
        ChangePasswordWithTokenCommand command, CancellationToken cancellationToken)
        => fromFlow
            .UpdateOne(x => x.Email == command.Email)
            .WithSpecialAction(null)
            .WithCondition(async user =>
            {
                var confirmTokenResult = await userRepository
                    .ConfirmEmailResetPassAsync(user, command.Token);
                return confirmTokenResult.MapT1(_ =>
                    IdentityErrorDetail.IdentityError.ResetUserPasswordFailed());
            })
            .WithModify(async user => await sender
                .Send(new ChangePasswordWithoutOldPasswordCommand(user.UserId, command.NewPassword),
                    cancellationToken))
            .WithErrorIfNull(IdentityErrorDetail.UserError.NotFound())
            .WithErrorIfSaveChange(
                IdentityErrorDetail.IdentityError.ResetUserPasswordFailed())
            .WithResultIfSucceed(user => new UserIdResponse { Id = user.UserId.ToString() });
}