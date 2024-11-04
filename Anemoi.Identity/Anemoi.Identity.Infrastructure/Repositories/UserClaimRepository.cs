using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.ModelIds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Domain.Models;
using Anemoi.Identity.Infrastructure.DataContext;

namespace Anemoi.Identity.Infrastructure.Repositories;

public class UserClaimRepository(
    UserManager<User> userManager,
    IdentityDbContext identityDbContext)
    : IUserClaimRepository
{
    public async Task<IList<Claim>> GetUserClaimsAsync(UserId userId,
        CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
        if (user is null) return new List<Claim>();
        var claims = await userManager.GetClaimsAsync(user);
        return claims;
    }

    public Task<List<Guid>> GetUserIdsByClaimTypes(List<string> claimTypes) =>
        identityDbContext
            .Set<IdentityUserClaim<Guid>>()
            .Where(a => claimTypes.Contains(a.ClaimType))
            .Select(a => a.UserId)
            .ToListAsync();

    public async Task<OneOf<None, ErrorDetail>> AddClaimsAsync(UserId userId, List<Claim> claims,
        CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
        if (user is null) return IdentityErrorDetail.UserError.NotFound();
        var setClaimResult = await userManager.AddClaimsAsync(user, claims);
        if (setClaimResult.Succeeded) return None.Value;
        return IdentityErrorDetail.UserError.UpdateFailed();
    }

    public async Task<OneOf<None, ErrorDetail>> RemoveClaimsAsync(UserId userId,
        List<string> claimTypes,
        CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
        if (user is null) return IdentityErrorDetail.UserError.NotFound();
        var existClaims = await userManager.GetClaimsAsync(user);
        var claimsRemoved = existClaims.Where(x => claimTypes.Contains(x.Type));
        var setClaimResult = await userManager.RemoveClaimsAsync(user, claimsRemoved);
        if (setClaimResult.Succeeded) return None.Value;
        return IdentityErrorDetail.UserError.UpdateFailed();
    }
}