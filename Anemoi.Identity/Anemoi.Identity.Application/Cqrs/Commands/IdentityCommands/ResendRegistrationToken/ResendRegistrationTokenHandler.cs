using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.IdentityCommands.ResendRegistrationToken;
using Anemoi.Contract.Identity.Errors;
using AutoMapper;
using Serilog;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.ResendRegistrationToken;

public sealed class ResendRegistrationTokenHandler(
    ISqlRepository<User> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandOneVoidHandler<User, ResendRegistrationTokenCommand>(sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<User> BuildCommand(
        IStartOneCommandVoid<User> fromFlow, ResendRegistrationTokenCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .UpdateOne(x => x.Email == command.Email)
            .WithSpecialAction(null)
            .WithCondition(user =>
            {
                if (user.IsActivated)
                    return IdentityErrorDetail.UserError.EmailConfirmed();
                return None.Value;
            })
            .WithModify(_ => { })
            .WithErrorIfNull(IdentityErrorDetail.UserError.NotFound())
            .WithErrorIfSaveChange(IdentityErrorDetail.UserError.UpdateFailed());
}