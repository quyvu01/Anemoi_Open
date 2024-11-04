using Anemoi.Contract.MasterData.ModelIds;
using Anemoi.MasterData.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anemoi.MasterData.Infrastructure.DataContext;

public sealed class ModelMapping :
    IEntityTypeConfiguration<Province>,
    IEntityTypeConfiguration<District>

{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new ProvinceId(id));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(512);
        builder.Property(x => x.Slug)
            .HasMaxLength(512);
        builder.Property(x => x.SearchHint)
            .HasMaxLength(512);
        builder.HasIndex(x => x.Slug);
    }

    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, id => new DistrictId(id));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(512);
        builder.HasOne(x => x.Province)
            .WithMany(x => x.Districts)
            .HasForeignKey(x => x.ProvinceId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.Property(x => x.Slug)
            .HasMaxLength(512);
        builder.Property(x => x.SearchHint)
            .HasMaxLength(512);
        builder.HasIndex(x => x.Slug);
    }
}