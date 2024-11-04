using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Configurations;
using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Application.IdentityResults;
using Microsoft.IdentityModel.Tokens;
using OneOf;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.TokenGenerators;

public sealed class TokenGeneratorHandler(
    SigningCredentials signingCredentials,
    JwtSetting jwtSetting,
    IUserRepository userRepository,
    IUserClaimRepository userClaimRepository)
    : ICommandHandler<TokenGeneratorCommand, OneOf<IdentitySuccess, ErrorDetail>>
{
    public async Task<OneOf<IdentitySuccess, ErrorDetail>> Handle(TokenGeneratorCommand request,
        CancellationToken cancellationToken)
    {
        var user = request.User;
        var tokenHandler = new JwtSecurityTokenHandler();
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, IdGenerator.NextGuid().ToString()));
        claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName ?? string.Empty));
        claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName ?? string.Empty));
        claimsIdentity.AddClaim(new Claim("id", user.UserId.ToString()));
        var userRoles = await userRepository.GetRolesAsync(user);
        claimsIdentity.AddClaims(userRoles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));
        var claims = await userClaimRepository
            .GetUserClaimsAsync(user.UserId, cancellationToken);
        claimsIdentity.AddClaims(claims);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.Add(jwtSetting.TokenLifetime),
            SigningCredentials = signingCredentials,
        };
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);
        return new IdentitySuccess
        {
            UserToken = token,
            TokenExpiryTime = tokenDescriptor.Expires,
            RefreshTokenExpiryTime = DateTime.UtcNow.Add(jwtSetting.RefreshTokenLifetime),
            JwtId = securityToken.Id,
            CreationDate = DateTime.UtcNow,
            UserId = user.UserId
        };
    }
};