using Anemoi.BuildingBlock.Infrastructure.GeneralInstaller;
using Anemoi.Secure.Infrastructure;
using Serilog;
using Serilog.Events;

var builder = Host.CreateDefaultBuilder(args);
builder.UseDefaultServiceProvider((context, provider) =>
    provider.ValidateScopes = provider.ValidateOnBuild = context.HostingEnvironment.IsDevelopment());

builder.ConfigureAppConfiguration(configuration => configuration
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .AddJsonFile("serilogConfiguration.json"));

builder.UseSerilog((host, configuration) => configuration.Enrich
    .FromLogContext()
    .ReadFrom.Configuration(host.Configuration)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning));

builder.ConfigureServices((context, service) =>
    service.InstallServicesInAssembly<ISecureInfrastructureAssemblyMarker>(context.Configuration));

using var host = builder.Build();

await host.RunAsync();