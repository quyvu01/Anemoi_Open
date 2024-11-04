using System.Reflection;
using Anemoi.BuildingBlock.Infrastructure.GeneralInstaller;
using Anemoi.BuildingBlock.Infrastructure.GeneralMiddlewares;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using Serilog.Events;
using Anemoi.Centralize.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider((context, provider) =>
{
    provider.ValidateScopes =
        provider.ValidateOnBuild =
            context.HostingEnvironment.IsDevelopment();
});

builder.Configuration
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .AddEnvironmentVariables()
    .AddJsonFile("serilogConfiguration.json");

builder.Host.UseSerilog((host, configuration) => configuration.Enrich
    .FromLogContext()
    .ReadFrom.Configuration(host.Configuration)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Information));

builder.Host.ConfigureServices((context, services) =>
{
    services.InstallServicesInAssembly<ICentralizeInfrastructureAssemblyMarker>(context.Configuration);
    services.AddHttpLogging(options
        => options.LoggingFields = HttpLoggingFields.All);
    services.AddRateLimiter(options =>
    {
        options.AddFixedWindowLimiter("FixedLimiter", opt =>
        {
            opt.Window = TimeSpan.FromSeconds(60);
            opt.PermitLimit = 3;
        });
        options.RejectionStatusCode = 429;
    });
});

var app = builder.Build();

if (builder.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseRateLimiter();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

await app.RunAsync();