using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.Secure.Application.Abstractions;
using Anemoi.Secure.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Secure.Infrastructure.Installers;

public sealed class ServiceInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IOtpService, OtpService>();
    }
}