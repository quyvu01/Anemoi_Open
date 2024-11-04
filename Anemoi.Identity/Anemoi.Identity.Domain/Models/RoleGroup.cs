using System;
using System.Collections.Generic;
using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Identity.ModelIds;

namespace Anemoi.Identity.Domain.Models;

public sealed class RoleGroup : ValueObject
{
    public RoleGroupId Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string SearchHint { get; set; }
    public DateTime CreatedTime { get; set; }
    public UserId CreatorId { get; set; }
    public User Creator { get; set; }
    public UserId UpdaterId { get; set; }
    public User Updater { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public bool IsDefault { get; set; }
    public List<RoleGroupMapRole> RoleGroupMapRoles { get; set; }
    public List<UserMapRoleGroup> UserMapRoleGroups { get; set; }
    public List<RoleGroupClaim> RoleGroupClaims { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}