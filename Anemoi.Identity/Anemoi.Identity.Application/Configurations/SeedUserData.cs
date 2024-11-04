using System.Collections.Generic;

namespace Anemoi.Identity.Application.Configurations;

public sealed class SeedUserData
{
    public List<SupperAdminUser> SupperAdminUsers { get; set; }
}
public sealed class SupperAdminUser
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}