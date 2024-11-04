using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Anemoi.BuildingBlock.Application.Abstractions;

namespace Anemoi.BuildingBlock.Infrastructure.GeneralInstaller;

public static class Extensions
{
    // By calling this method the program will find all the classes implemented the interface IInstaller
    public static void InstallServicesInAssembly<TAssembly>(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Find all implement IInstaller interface and then DI them
        var installers = typeof(TAssembly).Assembly.ExportedTypes.Where(x =>
                typeof(IInstaller).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false })
            .Select(Activator.CreateInstance).Cast<IInstaller>().ToList();
        installers.ForEach(installer => installer.InstallerServices(services, configuration));
    }
}