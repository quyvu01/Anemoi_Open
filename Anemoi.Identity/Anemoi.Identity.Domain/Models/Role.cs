using System;
using System.Collections.Generic;
using Anemoi.Contract.Identity.ModelIds;
using Microsoft.AspNetCore.Identity;

namespace Anemoi.Identity.Domain.Models;

public sealed class Role : IdentityRole<Guid>
{
    public RoleId RoleId { get; set; }
    public List<IdentityPolicyMapRole> IdentityPolicyMapRoles { get; set; }
}