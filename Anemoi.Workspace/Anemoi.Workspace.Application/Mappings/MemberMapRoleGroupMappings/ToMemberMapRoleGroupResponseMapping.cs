using Anemoi.Contract.Workspace.Responses;
using Anemoi.Workspace.Domain.Models;
using AutoMapper;

namespace Anemoi.Workspace.Application.Mappings.MemberMapRoleGroupMappings;

public sealed class ToMemberMapRoleGroupResponseMapping : Profile
{
    public ToMemberMapRoleGroupResponseMapping()
    {
        CreateMap<MemberMapRoleGroup, MemberMapRoleGroupResponse>();
    }
}