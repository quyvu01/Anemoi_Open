using Anemoi.BuildingBlock.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.BuildingBlock.Test;

public class ServicesBuilding
{
    protected IServiceProvider ServiceProvider { get; private set; }
    private readonly IServiceCollection _serviceCollection;
    private readonly List<IInstaller> _installers = [];
    private readonly List<Action<IServiceCollection, IConfiguration>> _serviceInstallers = [];
    private readonly IConfiguration _configuration;

    protected ServicesBuilding()
    {
        _serviceCollection = new ServiceCollection();
        _configuration = new ConfigurationBuilder()
            .Build();
    }

    private void BuildServiceProvider() => ServiceProvider = _serviceCollection.BuildServiceProvider();

    public ServicesBuilding InstallService<TInstaller>() where TInstaller : class, IInstaller
    {
        var serviceInstance = Activator.CreateInstance(typeof(TInstaller));
        _installers.Add(serviceInstance as IInstaller);
        return this;
    }

    public ServicesBuilding InstallService(Action<IServiceCollection, IConfiguration> installingAction)
    {
        _serviceInstallers.Add(installingAction);
        return this;
    }


    public void InstallAllServices()
    {
        _installers.ForEach(s => s.InstallerServices(_serviceCollection, _configuration));
        _serviceInstallers.ForEach(s => s.Invoke(_serviceCollection, _configuration));
        BuildServiceProvider();
    }
}