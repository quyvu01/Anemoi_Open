using System;
using Anemoi.Identity.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Anemoi.Identity.Infrastructure.DataContext;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options)
    : IdentityDbContext<User, Role, Guid>(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<RoleGroup> RoleGroups { get; set; }
    public DbSet<RoleGroupMapRole> RoleGroupMapRoles { get; set; }
    public DbSet<UserMapRoleGroup> UserMapRoleGroups { get; set; }
    public DbSet<IdentityPolicy> IdentityPolicies { get; set; }
    public DbSet<IdentityPolicyMapRole> IdentityPolicyMapRoles { get; set; }
    public DbSet<RoleGroupClaim> RoleGroupClaims { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IIdentityInfrastructureAssemblyMarker).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}