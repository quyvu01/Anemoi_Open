using System.Reflection;
using Anemoi.Contract.MasterData;
using Anemoi.Contract.Workspace;

namespace Anemoi.Centralize.Application.ContractAssemblies;

public static class ContractAssembly
{
    public static IEnumerable<Assembly> GetAllContractAssemblies() =>
    [
        typeof(IMasterDataContractAssemblyMarker).Assembly,
        typeof(IWorkspaceContractAssemblyMarker).Assembly
    ];
}