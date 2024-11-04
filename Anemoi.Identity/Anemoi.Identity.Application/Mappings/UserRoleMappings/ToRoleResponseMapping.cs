using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Domain.Models;
using AutoMapper;

namespace Anemoi.Identity.Application.Mappings.UserRoleMappings;

public sealed class ToRoleResponseMapping : Profile
{
    public ToRoleResponseMapping()
    {
        CreateMap<Role, UserRoleResponse>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.RoleId));
    }
}