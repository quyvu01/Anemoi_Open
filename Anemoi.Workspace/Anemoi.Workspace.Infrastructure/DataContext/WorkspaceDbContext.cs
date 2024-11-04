using Anemoi.Workspace.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Anemoi.Workspace.Infrastructure.DataContext;

public sealed class WorkspaceDbContext(DbContextOptions<WorkspaceDbContext> options) : DbContext(options)
{
    public DbSet<Domain.Models.Workspace> Workspaces { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<MemberMapOrganization> MemberMapOrganizations { get; set; }
    public DbSet<MemberInvitation> MemberInvitations { get; set; }
    public DbSet<MemberInvitationMapOrganization> MemberInvitationMapOrganizations { get; set; }
    public DbSet<MemberMapRoleGroup> MemberMapRoleGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IWorkspaceInfrastructureAssemblyMarker).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}