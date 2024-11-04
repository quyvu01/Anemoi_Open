using System;
using System.Collections.Generic;
using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Identity.ModelIds;

namespace Anemoi.Identity.Domain.Models;

public sealed class RefreshToken : ValueObject
{
    public RefreshTokenId Id { get; set; }
    public string UserToken { get; set; }
    public string JwtId { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime TokenExpiryTime { get; set; }
    public bool IsUsed { get; set; }
    public bool Invalidate { get; set; }
    public UserId UserId { get; set; }
    public User User { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return UserToken;
        yield return JwtId;
    }
}