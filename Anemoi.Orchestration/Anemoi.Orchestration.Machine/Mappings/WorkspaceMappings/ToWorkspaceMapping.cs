using Anemoi.Orchestration.Contract.WorkspaceContract.Events.WorkspaceInitializedEvents;
using Anemoi.Orchestration.Contract.WorkspaceContract.Instances;
using AutoMapper;

namespace Anemoi.Orchestrator.Machine.Mappings.WorkspaceMappings;

public sealed class ToWorkspaceMapping : Profile
{
    public ToWorkspaceMapping()
    {
        CreateMap<CreateWorkspaceInitialized, WorkspaceInitializedInstance>();
    }
}