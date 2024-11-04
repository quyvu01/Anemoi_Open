using System.Collections.Generic;
using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Identity.ModelIds;

namespace Anemoi.Identity.Domain.Models;

public class IdentityPolicyMapRole : ValueObject
{
    public IdentityPolicyMapRoleId Id { get; set; }
    public IdentityPolicyId IdentityPolicyId { get; set; }
    public IdentityPolicy IdentityPolicy { get; set; }
    public RoleId UserRoleId { get; set; }
    public Role Role { get; set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}