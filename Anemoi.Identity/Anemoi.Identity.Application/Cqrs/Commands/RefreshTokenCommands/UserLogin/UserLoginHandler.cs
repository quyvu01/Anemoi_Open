using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.RefreshTokenCommands.UserLogin;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.TokenGenerators;
using Anemoi.Identity.Application.IdentityResults;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OneOf;
using Serilog;
using SignInResult = Anemoi.Identity.Application.ApplicationModels.Enums.SignInResult;

namespace Anemoi.Identity.Application.Cqrs.Commands.RefreshTokenCommands.UserLogin;

public sealed class UserLoginHandler(
    IMapper mapper,
    ILogger logger,
    ISender sender,
    ISignInRepository signInRepository,
    ISqlRepository<RefreshToken> refreshTokenRepository,
    IUnitOfWork unitOfWork,
    ISqlRepository<User> userRepository,
    IPasswordHasher<User> passwordHasher)
    : EfCommandOneResultHandler<RefreshToken, UserLoginCommand, AuthenticationSuccessResponse>(refreshTokenRepository,
        unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderResult<RefreshToken, AuthenticationSuccessResponse> BuildCommand(
        IStartOneCommandResult<RefreshToken, AuthenticationSuccessResponse> fromFlow,
        UserLoginCommand command, CancellationToken cancellationToken)
        => fromFlow
            .CreateOne(new RefreshToken { Id = new RefreshTokenId(IdGenerator.NextGuid()) })
            .WithCondition(async refreshToken =>
            {
                var user = await userRepository
                    .GetFirstByConditionAsync(x => x.Email == command.UserName && x.IsActivated,
                        token: cancellationToken);
                if (user is null) return IdentityErrorDetail.UserError.NotFound();
                if (user.ChangedPasswordTime is null)
                    return IdentityErrorDetail.IdentityError.FirstTimePasswordWasNotChanged();
                if (user is { PasswordHash: null })
                    return IdentityErrorDetail.IdentityError.FirstTimePasswordWasNotChanged();
                var authResponse = await SignInAsync(user, command.Password);
                if (authResponse.IsT1) return authResponse.AsT1;
                Mapper.Map(authResponse.AsT0, refreshToken);
                return None.Value;
            })
            .WithErrorIfSaveChange(IdentityErrorDetail.IdentityError.LoginFailed())
            .WithResultIfSucceed(Mapper.Map<AuthenticationSuccessResponse>);

    private async Task<OneOf<IdentitySuccess, ErrorDetail>> SignInAsync(User user, string password)
    {
        var passwordCheckResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, password);
        if (passwordCheckResult == PasswordVerificationResult.Failed)
            return IdentityErrorDetail.IdentityError.PasswordNotCorrect();
        if (passwordCheckResult == PasswordVerificationResult.SuccessRehashNeeded)
            return IdentityErrorDetail.IdentityError.PasswordSuccessRehashNeeded();
        var signInResult = await signInRepository
            .PasswordSignInAsync(user, password, false, true);
        return signInResult switch
        {
            SignInResult.IsLockedOut => IdentityErrorDetail.IdentityError.UserLockedOut(),
            SignInResult.IsNotAllowed => IdentityErrorDetail.IdentityError.UserLogInNotAllowed(),
            SignInResult.RequiresTwoFactor => IdentityErrorDetail.IdentityError.UserRequiresTwoFactor(),
            _ => await sender.Send(new TokenGeneratorCommand(user))
        };
    }
}