using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.Contract.Identity.Commands.IdentityCommands.UpdateUserRoleGroup;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Identity.Domain.Models;
using AutoMapper;

namespace Anemoi.Identity.Application.Mappings.UserRoleGroupMappings;

public sealed class ToUserRoleGroupMapping : Profile
{
    public ToUserRoleGroupMapping()
    {
        CreateMap<UpdateUserRoleGroupCommand, UserMapRoleGroup>()
            .ForMember(x => x.Id, opt => opt.MapFrom(_ => new UserMapRoleGroupId(IdGenerator.NextGuid())))
            .ForMember(x => x.UserId, opt => opt.MapFrom(x => x.UserId));
    }
}