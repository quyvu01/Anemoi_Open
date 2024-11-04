using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anemoi.Identity.Infrastructure.DataContext;

public sealed class ModelMappings :
    IEntityTypeConfiguration<User>,
    IEntityTypeConfiguration<RefreshToken>,
    IEntityTypeConfiguration<Role>,
    IEntityTypeConfiguration<RoleGroup>,
    IEntityTypeConfiguration<RoleGroupClaim>,
    IEntityTypeConfiguration<RoleGroupMapRole>,
    IEntityTypeConfiguration<UserMapRoleGroup>,
    IEntityTypeConfiguration<IdentityPolicy>,
    IEntityTypeConfiguration<IdentityPolicyMapRole>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, id => new UserId(id));
        builder.HasKey(x => new { x.UserId });
        builder.Property(x => x.FirstName)
            .HasMaxLength(128);
        builder.Property(x => x.LastName)
            .HasMaxLength(128);
        builder.HasIndex(x => x.Email).IsUnique();
    }

    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new RefreshTokenId(id));
        builder.Ignore(x => x.TokenExpiryTime);
        builder.HasOne(x => x.User)
            .WithMany().HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => new { x.RoleId });
        builder.Property(x => x.RoleId)
            .HasConversion(x => x.Value, id => new RoleId(id));
    }

    public void Configure(EntityTypeBuilder<RoleGroup> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new RoleGroupId(id));
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Creator)
            .WithMany()
            .HasForeignKey(x => x.CreatorId)
            .HasPrincipalKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(x => x.Updater)
            .WithMany()
            .HasForeignKey(x => x.UpdaterId)
            .HasPrincipalKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    public void Configure(EntityTypeBuilder<RoleGroupMapRole> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new RoleGroupMapUserRoleId(id));
        builder.HasOne(x => x.RoleGroup)
            .WithMany(x => x.RoleGroupMapRoles).HasForeignKey(x => x.RoleGroupId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Role)
            .WithMany().HasForeignKey(x => x.RoleId)
            .HasPrincipalKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.RoleId).IsRequired();
        builder.Property(x => x.RoleGroupId).IsRequired();
    }

    public void Configure(EntityTypeBuilder<UserMapRoleGroup> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new UserMapRoleGroupId(id));

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserRoleGroups)
            .HasPrincipalKey(x => x.UserId)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RoleGroup)
            .WithMany(x => x.UserMapRoleGroups).HasForeignKey(x => x.RoleGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<IdentityPolicy> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new IdentityPolicyId(id));
    }

    public void Configure(EntityTypeBuilder<IdentityPolicyMapRole> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new IdentityPolicyMapRoleId(id));
        builder.HasOne(x => x.Role)
            .WithMany(x => x.IdentityPolicyMapRoles)
            .HasForeignKey(x => x.UserRoleId)
            .HasPrincipalKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.IdentityPolicy)
            .WithMany(x => x.IdentityPolicyMapRoles)
            .HasForeignKey(x => x.IdentityPolicyId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<RoleGroupClaim> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new RoleGroupClaimId(id));
        builder.HasIndex(a => new { a.Key, a.Value });
        builder.HasOne(a => a.RoleGroup)
            .WithMany(a => a.RoleGroupClaims)
            .HasForeignKey(a => a.RoleGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}