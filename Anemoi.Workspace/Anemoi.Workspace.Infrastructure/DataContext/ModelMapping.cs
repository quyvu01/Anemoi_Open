using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Workspace.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anemoi.Workspace.Infrastructure.DataContext;

public sealed class ModelMapping :
    IEntityTypeConfiguration<Organization>
{

    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new OrganizationId(id));
        builder.HasKey(x => x.Id);
        builder.HasOne(a => a.ParentOrganization)
            .WithMany(x => x.ChildOrganizations)
            .HasForeignKey(x => x.ParentOrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.SubDomain)
            .IsUnique();
    }
}