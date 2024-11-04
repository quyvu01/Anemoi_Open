using System.Reflection;
using Anemoi.BuildingBlock.Infrastructure.GeneralInstaller;
using Anemoi.BuildingBlock.Infrastructure.RunSqlMigration;
using Lambda.Identity.Application.SeedData;
using Anemoi.Identity.Infrastructure;
using Anemoi.Identity.Infrastructure.DataContext;
using Anemoi.Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider((context, provider) =>
    provider.ValidateScopes = provider.ValidateOnBuild = context.HostingEnvironment.IsDevelopment());

builder.Configuration
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .AddEnvironmentVariables()
    .AddJsonFile("serilogConfiguration.json")
    .AddJsonFile("defaultApplicationPolicy.json");

builder.Host.UseSerilog((host, configuration) => configuration.Enrich
    .FromLogContext()
    .ReadFrom.Configuration(host.Configuration)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning));

builder.Services.InstallServicesInAssembly<IIdentityInfrastructureAssemblyMarker>(builder.Configuration);

var app = builder.Build();

await MigrationDatabase.MigrationDatabaseAsync<IdentityDbContext>(app);

var serviceScope = app.Services.CreateScope();

await SeedData.SeedRolesAsync(serviceScope);

await SeedData.SeedApplicationPoliciesAsync(serviceScope);

await SeedData.RegisterAdministratorAsync(serviceScope);

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseRouting();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();

app.UseAuthorization();

app.MapGrpcService<IdentityGrpcService>();

app.MapGraphQL();

await app.RunAsync();