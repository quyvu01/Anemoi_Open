using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.IdentityCommands.UserLogout;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.UserLogout;

public sealed class UserLogoutHandler(
    ILogger logger,
    ISqlRepository<RefreshToken> refreshTokenRepository,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    ISignInRepository signInRepository)
    : EfCommandOneVoidHandler<RefreshToken, UserLogoutCommand>(refreshTokenRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<RefreshToken> BuildCommand(
        IStartOneCommandVoid<RefreshToken> fromFlow, UserLogoutCommand command,
        CancellationToken cancellationToken) => fromFlow
        .RemoveOne(x => x.UserToken == command.Token)
        .WithSpecialAction(null)
        .WithCondition(async _ =>
        {
            await signInRepository.SignOutAsync();
            return None.Value;
        }).WithErrorIfNull(IdentityErrorDetail.TokenError.RefreshTokenNotFound())
        .WithErrorIfSaveChange(IdentityErrorDetail.IdentityError.UserLogOutFailed());
}