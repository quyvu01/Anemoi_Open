using System.Reflection;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Configurations;
using Anemoi.Workspace.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Workspace.Infrastructure.Installers;

public sealed class DbInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        var databaseSetting = configuration.GetSection(nameof(PostgresDbSetting)).Get<PostgresDbSetting>()!;
        services.AddDbContextPool<WorkspaceDbContext>(options => options.UseNpgsql(databaseSetting.ConnectionString,
            builder =>
            {
                builder.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
            }), databaseSetting.PoolSize);
    }
}