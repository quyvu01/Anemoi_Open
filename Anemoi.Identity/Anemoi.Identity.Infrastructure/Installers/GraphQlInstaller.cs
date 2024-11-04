using Anemoi.BuildingBlock.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Anemoi.Identity.Application.GraphQl.Queries;
using Anemoi.Identity.Infrastructure.DataContext;

namespace Anemoi.Identity.Infrastructure.Installers;

public class GraphQlInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddGraphQLServer()
            .RegisterDbContext<IdentityDbContext>()
            .AddQueryType<IdentityQueries>()
            .AddProjections()
            .AddApolloTracing();
    }
}