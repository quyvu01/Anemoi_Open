using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.Contract.Workspace.Responses;
using Anemoi.Workspace.Application.GraphQl.Queries;
using Anemoi.Workspace.Infrastructure.DataContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Workspace.Infrastructure.Installers;

public class GraphQlInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddGraphQLServer()
            .RegisterDbContext<WorkspaceDbContext>()
            .AddQueryType<WorkspaceQueries>()
            .AddType<WorkspaceResponse>()
            .AddType(GraphQlAutoMapper.GraphQlAutoMapper.CreateGraphQlAutoMapper())
            .AddProjections()
            .AddApolloTracing();
    }
}