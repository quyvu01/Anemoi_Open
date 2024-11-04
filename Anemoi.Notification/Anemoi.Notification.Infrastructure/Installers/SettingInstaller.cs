using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Configurations;
using Anemoi.Notification.Application.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Anemoi.Notification.Application.ApplicationModels;

namespace Anemoi.Notification.Infrastructure.Installers;

public sealed class SettingInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection(nameof(DomainSetting)).Get<DomainSetting>());
        services.AddSingleton(configuration.GetSection(nameof(S3Setting)).Get<S3Setting>()!);
        services.AddSingleton(configuration.GetSection(nameof(EmailErrorMessages)).Get<EmailErrorMessages>()!);
    }
}