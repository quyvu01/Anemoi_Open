using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.BuildingBlock.Application.Abstractions;

public interface IInstaller
{
    void InstallerServices(IServiceCollection services, IConfiguration configuration);
}