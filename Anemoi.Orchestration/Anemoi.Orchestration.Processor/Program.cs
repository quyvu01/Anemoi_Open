using Anemoi.BuildingBlock.Infrastructure.GeneralInstaller;
using Anemoi.BuildingBlock.Infrastructure.RunSqlMigration;
using Serilog;
using Serilog.Events;
using Anemoi.Orchestrator.Infrastructure;
using Anemoi.Orchestrator.Infrastructure.DataContext;


var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureAppConfiguration(configuration => configuration
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .AddJsonFile("serilogConfiguration.json"));

var host = builder.ConfigureServices((hostContext, services) =>
        services.InstallServicesInAssembly<IOrchestrationInfrastructureAssemblyMarker>(hostContext
            .Configuration))
    .UseSerilog((host, configuration) => configuration.Enrich
        .FromLogContext()
        .ReadFrom.Configuration(host.Configuration)
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .MinimumLevel.Override("System", LogEventLevel.Information)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)).Build();

await MigrationDatabase.MigrationDatabaseAsync<OrchestrationDbContext>(host);
await host.RunAsync();