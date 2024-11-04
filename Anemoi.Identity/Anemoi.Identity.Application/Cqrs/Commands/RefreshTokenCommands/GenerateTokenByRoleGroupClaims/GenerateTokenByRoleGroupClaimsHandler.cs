using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.RefreshTokenCommands.GenerateTokenByRoleGroupClaims;
using Anemoi.Contract.Identity.Contracts;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Contract.Identity.Queries.RoleQueries.GetUserRolesByRoleGroupClaims;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OneOf;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Commands.RefreshTokenCommands.GenerateTokenByRoleGroupClaims;

public sealed class GenerateTokenByRoleGroupClaimsHandler(
    SigningCredentials signingCredentials,
    TokenValidationParameters tokenValidationParameters,
    ILogger logger,
    IMapper mapper,
    ISqlRepository<User> userRepository,
    ISqlRepository<RefreshToken> sqlRepository,
    IUnitOfWork unitOfWork,
    ISender sender,
    ITokenGetter tokenGetter,
    IUserClaimRepository userClaimRepository)
    : EfCommandOneResultHandler<RefreshToken, GenerateTokenByRoleGroupClaimsCommand, AuthenticationSuccessResponse>(
        sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderResult<RefreshToken, AuthenticationSuccessResponse> BuildCommand(
        IStartOneCommandResult<RefreshToken, AuthenticationSuccessResponse> fromFlow,
        GenerateTokenByRoleGroupClaimsCommand command, CancellationToken cancellationToken)
        => fromFlow
            .CreateOne(new RefreshToken { Id = new RefreshTokenId(IdGenerator.NextGuid()) })
            .WithCondition(async refreshToken =>
            {
                var userId = new UserId(Guid.Parse(tokenGetter.Token.GetUserIdFromToken()));
                var user = await userRepository
                    .GetFirstByConditionAsync(x => x.UserId == userId && x.IsActivated, db => db.AsNoTracking(),
                        cancellationToken);
                if (user is null) return IdentityErrorDetail.UserError.NotFound();
                if (user.ChangedPasswordTime is null)
                    return IdentityErrorDetail.IdentityError.FirstTimePasswordWasNotChanged();
                if (user is { PasswordHash: null })
                    return IdentityErrorDetail.IdentityError.FirstTimePasswordWasNotChanged();
                var tokenResponse =
                    await GenerateWorkspaceTokenAsync(user, command.RoleGroupClaims, cancellationToken);
                if (tokenResponse.IsT1) return tokenResponse.AsT1;
                refreshToken.UserToken = tokenResponse.AsT0;
                return None.Value;
            })
            .WithErrorIfSaveChange(IdentityErrorDetail.IdentityError.LoginFailed())
            .WithResultIfSucceed(Mapper.Map<AuthenticationSuccessResponse>);

    private async Task<OneOf<string, ErrorDetail>> GenerateWorkspaceTokenAsync(User user,
        List<RoleGroupClaimContract> RoleGroupClaims, CancellationToken cancellationToken)
    {
        var validateTokenResult = ValidateToken(tokenGetter.Token, out var expiredTime);
        if (validateTokenResult.IsT1) return validateTokenResult.AsT1;
        var tokenHandler = new JwtSecurityTokenHandler();
        var claimsIdentity = new ClaimsIdentity();
        var rolesResult = await sender
            .Send(new GetUserRolesByRoleGroupClaimsQuery(user.UserId, RoleGroupClaims), cancellationToken);
        var claims = await userClaimRepository
            .GetUserClaimsAsync(user.UserId, cancellationToken);
        claimsIdentity.AddClaims(claims);
        var roles = rolesResult.Items.SelectMany(x => x.IdentityRoles).Select(a => a.Name).ToList();
        var roleGroupClaims = rolesResult.Items.SelectMany(a => a.RoleGroupClaims);
        claimsIdentity.AddClaim(new Claim("id", user.UserId.ToString()));
        claimsIdentity.AddClaims(roleGroupClaims.Select(a => new Claim(a.Key, a.Value)));
        claimsIdentity.AddClaims(RoleGroupClaims.Select(a => new Claim(a.Key, a.Value)));
        claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = expiredTime,
            SigningCredentials = signingCredentials,
        };
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);
        return token;
    }

    private OneOf<ClaimsPrincipal, ErrorDetail> ValidateToken(string token, out DateTime? expiredTime)
    {
        expiredTime = null;
        var validateToken = GetPrincipalFromToken(token);
        if (validateToken is null) return IdentityErrorDetail.TokenError.InvalidToken();

        if (!long.TryParse(validateToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value,
                out var expiryDateUnix)) return IdentityErrorDetail.TokenError.InvalidToken();

        expiredTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);
        // if (expiredTime < DateTime.UtcNow) return IdentityErrorDetail.TokenError.TokenExpired();
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