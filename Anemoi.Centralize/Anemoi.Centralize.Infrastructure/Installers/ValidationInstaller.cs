using Anemoi.BuildingBlock.Application.Abstractions;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Anemoi.Centralize.Application.ContractAssemblies;

namespace Anemoi.Centralize.Infrastructure.Installers;

public sealed class ValidationInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssemblies(ContractAssembly.GetAllContractAssemblies());
    }
}