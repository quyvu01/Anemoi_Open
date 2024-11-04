using System;
using Anemoi.Contract.Identity.ModelIds;

namespace Anemoi.Identity.Application.IdentityResults;

public sealed class IdentitySuccess
{
    public string UserToken { get; set; }
    public DateTime? TokenExpiryTime { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string JwtId { get; set; }
    public DateTime CreationDate { get; set; }
    public UserId UserId { get; set; }
}