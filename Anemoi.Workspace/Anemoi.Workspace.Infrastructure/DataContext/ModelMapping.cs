using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Workspace.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anemoi.Workspace.Infrastructure.DataContext;

public sealed class ModelMapping :
    IEntityTypeConfiguration<Domain.Models.Workspace>,
    IEntityTypeConfiguration<Organization>,
    IEntityTypeConfiguration<Member>,
    IEntityTypeConfiguration<MemberMapOrganization>,
    IEntityTypeConfiguration<MemberInvitation>,
    IEntityTypeConfiguration<MemberInvitationMapOrganization>,
    IEntityTypeConfiguration<MemberMapRoleGroup>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Workspace> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new WorkspaceId(id));
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.UserId);
    }

    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new OrganizationId(id));
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Workspace)
            .WithMany(x => x.Organizations)
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(a => a.ParentOrganization)
            .WithMany(x => x.ChildOrganizations)
            .HasForeignKey(x => x.ParentOrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.SubDomain)
            .IsUnique();
    }


    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new MemberId(id));
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Workspace)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<MemberMapOrganization> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new MemberMapOrganizationId(id));
        builder.HasKey(x => x.Id);
        builder.HasOne(a => a.Organization)
            .WithMany(x => x.MemberMapOrganizations)
            .HasForeignKey(a => a.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(a => a.Member)
            .WithMany(a => a.MemberMapOrganizations)
            .HasForeignKey(a => a.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<MemberInvitation> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new MemberInvitationId(id));
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Email, x.WorkspaceId }).IsUnique();
        builder.HasOne(x => x.Workspace)
            .WithMany(x => x.MemberInvitations)
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<MemberInvitationMapOrganization> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new MemberInvitationMapOrganizationId(id));
        builder.HasKey(x => x.Id);
        builder.HasOne(a => a.Organization)
            .WithMany()
            .HasForeignKey(a => a.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(a => a.MemberInvitation)
            .WithMany(a => a.MemberInvitationMapOrganizations)
            .HasForeignKey(a => a.MemberInvitationId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<MemberMapRoleGroup> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new MemberMapRoleGroupId(id));
        builder.HasKey(x => x.Id);
        builder.HasOne(a => a.Member)
            .WithMany(a => a.MemberMapRoleGroups)
            .HasForeignKey(a => a.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}