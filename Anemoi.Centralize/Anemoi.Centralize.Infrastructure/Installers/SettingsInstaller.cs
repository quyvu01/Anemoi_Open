using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Anemoi.Centralize.Application.Configurations;

namespace Anemoi.Centralize.Infrastructure.Installers;

public sealed class SettingsInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection(nameof(S3Setting)).Get<S3Setting>()!);
        services.AddSingleton(configuration.GetSection(nameof(GrpcSetting)).Get<GrpcSetting>()!);
    }
}