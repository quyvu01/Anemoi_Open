using System.Collections.Generic;
using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Identity.ModelIds;

namespace Anemoi.Identity.Domain.Models;

public class IdentityPolicy : ValueObject
{
    public IdentityPolicyId Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public List<IdentityPolicyMapRole> IdentityPolicyMapRoles { get; set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}