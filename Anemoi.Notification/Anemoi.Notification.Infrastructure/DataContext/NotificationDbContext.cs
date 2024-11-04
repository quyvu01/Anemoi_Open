using Anemoi.Notification.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Anemoi.Notification.Infrastructure.DataContext;

public sealed class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : DbContext(options)
{
    public DbSet<EmailConfiguration> EmailConfigurations { get; set; }
    public DbSet<Domain.Models.Notification> Notifications { get; set; }
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(INotificationInfrastructureAssemblyMarker).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}