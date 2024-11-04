using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.Secure.Application.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Secure.Infrastructure.Installers;

public sealed class SettingInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection(nameof(OtpSetting)).Get<OtpSetting>()!);
    }
}