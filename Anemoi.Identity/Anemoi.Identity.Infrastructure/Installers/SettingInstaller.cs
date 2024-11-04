using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.Identity.Application.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Identity.Infrastructure.Installers;

public sealed class SettingInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection(nameof(PasswordConfiguration)).Get<PasswordConfiguration>()!);
        services.AddSingleton(configuration.GetSection(nameof(SeedUserData)).Get<SeedUserData>()!);
        services.AddSingleton(configuration.GetSection(nameof(DefaultApplicationPolices)).Get<DefaultApplicationPolices>()!);
    }
}