using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.Contract.Workspace.Commands.MemberCommands.CreateMember;
using Anemoi.Contract.Workspace.ModelIds;
using AutoMapper;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Mappings.MemberMappings;

public class ToMemberMapping : Profile
{
    public ToMemberMapping()
    {
        CreateMap<CreateMemberCommand, Member>()
            .ForMember(x => x.Id, opt => opt.MapFrom(_ => new MemberId(IdGenerator.NextGuid())))
            .ForMember(x => x.CreatedTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(x => x.IsActivated, opt => opt.MapFrom(_ => true))
            .AfterMap((src, des) =>
            {
                if (src.RoleGroupIds is { Count: > 0 } roleGroupIds)
                    des.MemberMapRoleGroups = roleGroupIds.Select((a, index) => new MemberMapRoleGroup
                    {
                        Id = new MemberMapRoleGroupId(IdGenerator.NextGuid()), RoleGroupId = a, Order = index
                    }).ToList();
            });

        CreateMap<Member, Member>()
            .ForMember(x => x.Id, opt => opt.MapFrom(_ => new MemberId(IdGenerator.NextGuid())));
    }
}