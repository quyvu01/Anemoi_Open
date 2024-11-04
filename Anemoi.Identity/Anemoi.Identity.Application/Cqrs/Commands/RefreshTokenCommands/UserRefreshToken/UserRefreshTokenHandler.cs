using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.RefreshTokenCommands.UserRefreshToken;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.TokenGenerators;
using Anemoi.Identity.Application.IdentityResults;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OneOf;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Commands.RefreshTokenCommands.UserRefreshToken;

public sealed class UserRefreshTokenHandler(
    IMapper mapper,
    ILogger logger,
    TokenValidationParameters tokenValidationParameters,
    ISqlRepository<RefreshToken> sqlRepository,
    ISender sender,
    IUnitOfWork unitOfWork,
    ISqlRepository<User> userDbRepository)
    : EfCommandOneResultHandler<RefreshToken, UserRefreshTokenCommand, AuthenticationSuccessResponse>(
        sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderResult<RefreshToken, AuthenticationSuccessResponse> BuildCommand(
        IStartOneCommandResult<RefreshToken, AuthenticationSuccessResponse> fromFlow,
        UserRefreshTokenCommand command, CancellationToken cancellationToken)
        => fromFlow
            .CreateOne(new RefreshToken { Id = new RefreshTokenId(IdGenerator.NextGuid()) })
            .WithCondition(async refreshToken =>
            {
                var oldRefreshToken = await SqlRepository
                    .GetFirstByConditionAsync(x => x.Id == command.RefreshToken, token: cancellationToken);
                if (oldRefreshToken is null) return IdentityErrorDetail.TokenError.RefreshTokenNotFound();
                var authResult = await RefreshTokenAsync(oldRefreshToken, cancellationToken);
                return authResult.MapT0(success =>
                {
                    oldRefreshToken.IsUsed = true;
                    Mapper.Map(success, refreshToken);
                    return None.Value;
                });
            })
            .WithErrorIfSaveChange(IdentityErrorDetail.TokenError.CreateRefreshTokenFailed())
            .WithResultIfSucceed(Mapper.Map<AuthenticationSuccessResponse>);

    private async Task<OneOf<IdentitySuccess, ErrorDetail>> RefreshTokenAsync(RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        var validateTokenResult = ValidateToken(refreshToken.UserToken);
        if (validateTokenResult.IsT1) return validateTokenResult.AsT1;
        var user = await userDbRepository.GetFirstByConditionAsync(x =>
                x.UserId == refreshToken.UserId && x.IsActivated,
            db => db.AsNoTracking(), cancellationToken);
        if (user is null) IdentityErrorDetail.UserError.NotFound();
        if (DateTime.UtcNow > refreshToken.ExpiryDate)
            return IdentityErrorDetail.TokenError.SessionExpired();
        if (refreshToken.Invalidate)
            return IdentityErrorDetail.TokenError.InvalidRefreshToken();
        if (refreshToken.IsUsed)
            return IdentityErrorDetail.TokenError.RefreshTokenUsed();
        var authenticationResult = await sender
            .Send(new TokenGeneratorCommand(user), cancellationToken);
        return authenticationResult;
    }

    private OneOf<ClaimsPrincipal, ErrorDetail> ValidateToken(string token)
    {
        var validateToken = GetPrincipalFromToken(token);
        if (validateToken is null) return IdentityErrorDetail.TokenError.InvalidToken();

        if (!long.TryParse(validateToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value,
                out var expiryDateUnix)) return IdentityErrorDetail.TokenError.InvalidToken();

        var expiryTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);
        if (expiryTimeUtc > DateTime.UtcNow)
            return IdentityErrorDetail.TokenError.TokenIsNotExpired();
        return validateToken;
    }

    private ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            return !IsJwtWithValidateSecurityAlgorithm(validatedToken) ? null : principal;
        }
        catch (Exception ex)
        {
            Logger.Information("Error while get principal from token: {Error}", ex.Message);
            return null;
        }
    }

    private static bool IsJwtWithValidateSecurityAlgorithm(SecurityToken validateToken) =>
        validateToken is JwtSecurityToken jwtSecurityToken &&
        jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256,
            StringComparison.InvariantCultureIgnoreCase);
}