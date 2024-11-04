using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.Contract.Identity.ModelIds;
using OneOf;

namespace Anemoi.Identity.Application.Abstractions;

public interface IUserClaimRepository
{
    Task<IList<Claim>> GetUserClaimsAsync(UserId userId, CancellationToken cancellationToken);

    Task<List<Guid>> GetUserIdsByClaimTypes(List<string> claimTypes);

    Task<OneOf<None, ErrorDetail>> AddClaimsAsync(UserId userId, List<Claim> claims,
        CancellationToken cancellationToken);

    Task<OneOf<None, ErrorDetail>> RemoveClaimsAsync(UserId userId, List<string> claimTypes,
        CancellationToken cancellationToken);
}