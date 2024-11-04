using Anemoi.BuildingBlock.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Anemoi.Centralize.Application.Filters;

namespace Anemoi.Centralize.Infrastructure.Installers;

public sealed class ControllerInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(c => c.Filters.Add<AutoMapDataFilter>())
            .AddNewtonsoftJson(opts => opts.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
    }
}