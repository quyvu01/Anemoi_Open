using System.Collections.Generic;
using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Identity.ModelIds;

namespace Anemoi.Identity.Domain.Models;

public sealed class RoleGroupClaim : ValueObject
{
    public RoleGroupClaimId Id { get; set; }
    public RoleGroupId RoleGroupId { get; set; }
    public RoleGroup RoleGroup { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}