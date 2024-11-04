using System.Reflection;
using Anemoi.BuildingBlock.Infrastructure.GeneralInstaller;
using Anemoi.BuildingBlock.Infrastructure.RunSqlMigration;
using Anemoi.Notification.Infrastructure;
using Anemoi.Notification.Infrastructure.DataContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .AddEnvironmentVariables()
    .AddJsonFile("serilogConfiguration.json");

builder.Host.UseSerilog((host, configuration) => configuration.Enrich
    .FromLogContext()
    .ReadFrom.Configuration(host.Configuration)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning));

builder.Services.InstallServicesInAssembly<INotificationInfrastructureAssemblyMarker>(builder.Configuration);

var app = builder.Build();

await MigrationDatabase.MigrationDatabaseAsync<NotificationDbContext>(app);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseRouting();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

await app.RunAsync();