using Anemoi.Contract.Workspace.Responses;
using AutoMapper;

namespace Anemoi.Workspace.Application.Mappings.WorkspaceMappings;

public sealed class ToWorkspaceResponseMapping : Profile
{
    public ToWorkspaceResponseMapping()
    {
        CreateMap<Anemoi.Workspace.Domain.Models.Workspace, WorkspaceResponse>();
    }
}