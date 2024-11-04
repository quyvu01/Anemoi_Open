using Anemoi.MasterData.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Anemoi.MasterData.Infrastructure.DataContext;

public sealed class MasterDataDbContext(DbContextOptions<MasterDataDbContext> options) : DbContext(options)
{
    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> Districts { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IMasterDataInfrastructureAssemblyMarker).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}