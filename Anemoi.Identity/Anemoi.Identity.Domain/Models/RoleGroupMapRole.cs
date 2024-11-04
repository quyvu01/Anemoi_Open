using System.Collections.Generic;
using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Identity.ModelIds;

namespace Anemoi.Identity.Domain.Models;

public sealed class RoleGroupMapRole : ValueObject
{
    public RoleGroupMapUserRoleId Id { get; set; }
    public RoleGroupId RoleGroupId { get; set; }
    public RoleGroup RoleGroup { get; set; }
    public RoleId RoleId { get; set; }
    public Role Role { get; set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}