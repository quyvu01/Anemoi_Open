using System.Collections.Generic;
using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Identity.ModelIds;

namespace Anemoi.Identity.Domain.Models;

public sealed class UserMapRoleGroup : ValueObject
{
    public UserMapRoleGroupId Id { get; set; }
    public UserId UserId { get; set; }
    public User User { get; set; }
    public RoleGroupId RoleGroupId { get; set; }
    public RoleGroup RoleGroup { get; set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}