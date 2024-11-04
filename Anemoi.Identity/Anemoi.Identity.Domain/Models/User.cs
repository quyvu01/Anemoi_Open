using System;
using System.Collections.Generic;
using Anemoi.Contract.Identity.ModelIds;
using Microsoft.AspNetCore.Identity;

namespace Anemoi.Identity.Domain.Models;

public sealed class User : IdentityUser<Guid>
{
    public UserId UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string SearchHint { get; set; }
    public string Avatar { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? ChangedPasswordTime { get; set; }
    public bool IsActivated { get; set; }
    public bool IsRemoved { get; set; }
    public List<UserMapRoleGroup> UserRoleGroups { get; set; }
}