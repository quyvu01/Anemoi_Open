using Anemoi.BuildingBlock.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Notification.Infrastructure.Installers;

public sealed class PolicyInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options => options
            .AddPolicy("CorsPolicy", builder => builder
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()));
    }
}