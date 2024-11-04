using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.IdentityCommands.ConfirmEmail;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Identity.Application.Abstractions;
using AutoMapper;
using Serilog;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.ConfirmEmail;

public sealed class ConfirmEmailHandler(
    ISqlRepository<User> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger,
    IUserRepository userRepository)
    : EfCommandOneVoidHandler<User, ConfirmEmailCommand>(sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<User> BuildCommand(
        IStartOneCommandVoid<User> fromFlow, ConfirmEmailCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .UpdateOne(x => x.Email == command.Email)
            .WithSpecialAction(null)
            .WithCondition(async user =>
            {
                if (user.IsActivated)
                    return IdentityErrorDetail.UserError.EmailConfirmed();
                var confirmResult = await userRepository
                    .ConfirmEmailAsync(user, command.Token);
                return confirmResult.MapT1(_ => IdentityErrorDetail.UserError.UpdateFailed());
            })
            .WithModify(user => user.IsActivated = true)
            .WithErrorIfNull(IdentityErrorDetail.UserError.NotFound())
            .WithErrorIfSaveChange(IdentityErrorDetail.UserError.UpdateFailed());
}