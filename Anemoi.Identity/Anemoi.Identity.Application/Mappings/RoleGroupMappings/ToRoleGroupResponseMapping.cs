using Anemoi.Contract.Identity.Responses;
using AutoMapper;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.Mappings.RoleGroupMappings;

public class ToRoleGroupResponseMapping : Profile
{
    public ToRoleGroupResponseMapping()
    {
        CreateMap<RoleGroup, RoleGroupResponse>()
            .ForMember(x => x.IdentityRoles, opt => opt.MapFrom(x => x.RoleGroupMapRoles));

        CreateMap<RoleGroup, RoleGroupsResponse>()
            .ForMember(x => x.CreatorId, opt => opt.MapFrom(x => x.CreatorId == null ? null : x.CreatorId.ToString()))
            .ForMember(x => x.CreatorName, opt => opt.MapFrom(x => x.CreatorId == null ? null : x.Creator.FirstName))
            .ForMember(x => x.CreatorEmail, opt => opt.MapFrom(x => x.CreatorId == null ? null : x.Creator.Email))
            .ForMember(x => x.UpdaterId, opt => opt.MapFrom(x => x.Updater == null ? null : x.UpdaterId.ToString()))
            .ForMember(x => x.UpdaterName, opt => opt.MapFrom(x => x.Updater == null ? null : x.Updater.FirstName))
            .ForMember(x => x.UpdaterEmail, opt => opt.MapFrom(x => x.Updater == null ? null : x.Updater.Email));

        CreateMap<UserMapRoleGroup, UserRoleGroupResponse>();

        CreateMap<RoleGroupMapRole, UserRoleResponse>()
            .ForMember(x => x.Id,
                opt => opt.MapFrom(x => x.RoleId != null ? x.RoleId.ToString() : null))
            .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Role.Name));
    }
}