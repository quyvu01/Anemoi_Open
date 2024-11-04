using System.Reflection;
using Anemoi.Contract.Identity;
using Anemoi.Contract.MasterData;
using Anemoi.Contract.Notification;
using Anemoi.Contract.Workspace;

namespace Anemoi.Centralize.Application.ContractAssemblies;

public static class ContractAssembly
{
    public static IEnumerable<Assembly> GetAllContractAssemblies() =>
    [
        typeof(IIdentityContractAssemblyMarker).Assembly,
        typeof(IMasterDataContractAssemblyMarker).Assembly,
        typeof(INotificationContractAssemblyMarker).Assembly,
        typeof(IWorkspaceContractAssemblyMarker).Assembly
    ];
}