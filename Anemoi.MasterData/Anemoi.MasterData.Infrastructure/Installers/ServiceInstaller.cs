using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Infrastructure.GeneralInstaller;
using Anemoi.MasterData.Domain;
using Anemoi.MasterData.Infrastructure.DataContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.MasterData.Infrastructure.Installers;

public sealed class ServiceInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEfRepositoriesAsScope<MasterDataDbContext>(typeof(IMasterDataDomainAssemblyMarker).Assembly);
        services.AddEfUnitOfWorkAsScope<MasterDataDbContext>();
    }
}