using Anemoi.BuildingBlock.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Identity.Infrastructure.Installers;

public sealed class GrpcInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc();
    }
}