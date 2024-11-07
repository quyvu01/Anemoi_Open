using Anemoi.Workspace.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Anemoi.Workspace.Infrastructure.DataContext;

public sealed class WorkspaceDbContext(DbContextOptions<WorkspaceDbContext> options) : DbContext(options)
{
    public DbSet<Organization> Organizations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IWorkspaceInfrastructureAssemblyMarker).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}