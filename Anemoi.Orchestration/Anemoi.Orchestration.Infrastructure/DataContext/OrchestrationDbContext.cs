using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Anemoi.Orchestrator.Infrastructure.DataContext;

public sealed class OrchestrationDbContext(DbContextOptions<OrchestrationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}