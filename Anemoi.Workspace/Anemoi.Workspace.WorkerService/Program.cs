using System.Reflection;
using Anemoi.BuildingBlock.Infrastructure.GeneralInstaller;
using Anemoi.BuildingBlock.Infrastructure.RunSqlMigration;
using Anemoi.Workspace.Infrastructure;
using Anemoi.Workspace.Infrastructure.DataContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseDefaultServiceProvider((context, provider) =>
    provider.ValidateScopes = provider.ValidateOnBuild = context.HostingEnvironment.IsDevelopment());

builder.Configuration
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .AddEnvironmentVariables()
    .AddJsonFile("serilogConfiguration.json");

builder.Host.UseSerilog((host, configuration) => configuration.Enrich
    .FromLogContext()
    .ReadFrom.Configuration(host.Configuration)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Information)
    // .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
);

builder.Services.InstallServicesInAssembly<IWorkspaceInfrastructureAssemblyMarker>(builder.Configuration);

var app = builder.Build();

await MigrationDatabase.MigrationDatabaseAsync<WorkspaceDbContext>(app);

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.MapGraphQL();

await app.RunAsync();