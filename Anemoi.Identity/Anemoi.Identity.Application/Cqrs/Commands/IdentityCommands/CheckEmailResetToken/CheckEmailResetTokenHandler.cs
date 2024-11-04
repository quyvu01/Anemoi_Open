using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.IdentityCommands.CheckEmailResetTokenCommand;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Identity.Application.Abstractions;
using AutoMapper;
using Serilog;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.CheckEmailResetToken;

public sealed class CheckEmailResetTokenHandler(
    ISqlRepository<User> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger,
    IUserRepository userRepository)
    : EfCommandOneVoidHandler<User, CheckEmailResetTokenCommand>(sqlRepository,
        unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<User> BuildCommand(
        IStartOneCommandVoid<User> fromFlow, CheckEmailResetTokenCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .UpdateOne(x => x.Email == command.Email && x.IsActivated && !x.IsRemoved)
            .WithSpecialAction(null)
            .WithCondition(async user =>
            {
                var emailValidResult = await userRepository
                    .ConfirmEmailResetPassAsync(user, command.Code);
                return emailValidResult.MapT1(_ => IdentityErrorDetail.IdentityError.UserEmailWasNotVerified());
            })
            .WithModify(_ => { })
            .WithErrorIfNull(IdentityErrorDetail.UserError.NotFound())
            .WithErrorIfSaveChange(IdentityErrorDetail.UserError.UpdateFailed());
}