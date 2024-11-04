using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Anemoi.BuildingBlock.Infrastructure.RunSqlMigration;

public static class MigrationDatabase
{
    public static async Task MigrationDatabaseAsync<T>(IHost host) where T : DbContext
    {
        using var serviceScope = host.Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<T>();
        var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger>();
        try
        {
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (!pendingMigrations.Any()) return;
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            logger.Error("Error while migrations {@DbName} Database: {@Message}!", typeof(T).Name, e.Message);
        }
    }
}