using Anemoi.Contract.Notification.ModelIds;
using Anemoi.Notification.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anemoi.Notification.Infrastructure.DataContext;

public class ModelMapping :
    IEntityTypeConfiguration<EmailConfiguration>,
    IEntityTypeConfiguration<Domain.Models.Notification>,
    IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<EmailConfiguration> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new EmailConfigurationId(id));
        builder.HasKey(x => x.Id);
        builder.HasIndex(a => a.WorkspaceId);
    }

    public void Configure(EntityTypeBuilder<Domain.Models.Notification> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new NotificationId(id));
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.TargetUserId);
        builder.HasIndex(x => new { x.TargetUserId, x.WorkspaceId });
    }

    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new NotificationTemplateId(id));
        builder.HasKey(x => x.Id);
    }
}