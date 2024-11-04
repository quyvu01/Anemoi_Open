using System.Reflection;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Configurations;
using Anemoi.MasterData.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.MasterData.Infrastructure.Installers;

public sealed class DbInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        var databaseSetting = configuration.GetSection(nameof(PostgresDbSetting)).Get<PostgresDbSetting>()!;
        services.AddDbContextPool<MasterDataDbContext>(options => options.UseNpgsql(databaseSetting.ConnectionString,
            builder =>
            {
                builder.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            }), databaseSetting.PoolSize);
    }
}