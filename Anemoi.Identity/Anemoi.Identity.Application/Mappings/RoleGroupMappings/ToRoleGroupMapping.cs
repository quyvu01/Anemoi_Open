using System;
using System.Linq;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.Contract.Identity.Commands.RoleGroupCommands.CreateRoleGroup;
using Anemoi.Contract.Identity.Commands.RoleGroupCommands.UpdateRoleGroup;
using Anemoi.Contract.Identity.ModelIds;
using AutoMapper;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.Mappings.RoleGroupMappings;

public class ToRoleGroupMapping : Profile
{
    public ToRoleGroupMapping()
    {
        CreateMap<CreateRoleGroupCommand, RoleGroup>()
            .ForMember(x => x.Id, opt => opt.MapFrom(_ => new RoleGroupId(IdGenerator.NextGuid())))
            .ForMember(x => x.CreatedTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(x => x.CreatorId,
                opt => opt.MapFrom<UserValueResolver<CreateRoleGroupCommand>>())
            .ForMember(x => x.SearchHint, opt => opt.MapFrom(x => x.Name.GenerateSearchHint()))
            .ForMember(x => x.RoleGroupClaims, opt => opt.Ignore())
            .AfterMap((src, des) =>
            {
                if (src.IdentityRoleIds is { Count: > 0 } identityRoleIds)
                    des.RoleGroupMapRoles = identityRoleIds.Select(roleId => new RoleGroupMapRole
                    {
                        Id = new RoleGroupMapUserRoleId(IdGenerator.NextGuid()),
                        RoleGroupId = des.Id,
                        RoleId = roleId
                    }).ToList();
                if (src.RoleGroupClaims is { Count: > 0 } claims)
                    des.RoleGroupClaims = claims.Select(claim => new RoleGroupClaim
                    {
                        Id = new RoleGroupClaimId(IdGenerator.NextGuid()), Key = claim.Key, Value = claim.Value
                    }).ToList();
            });


        CreateMap<UpdateRoleGroupCommand, RoleGroup>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.RoleGroupMapRoles, opt => opt.Ignore())
            .ForMember(x => x.RoleGroupClaims, opt => opt.Ignore())
            .ForMember(x => x.UpdatedTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(x => x.UpdaterId,
                opt => opt.MapFrom<UserValueResolver<UpdateRoleGroupCommand>>())
            .ForMember(x => x.SearchHint, opt =>
            {
                opt.PreCondition(x => x.Name is { });
                opt.MapFrom(x => x.Name.GenerateSearchHint());
            })
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember is { }));
    }

    private class UserValueResolver<TCommand>(IUserIdGetter userIdGetter) :
        IValueResolver<TCommand, RoleGroup, UserId>
    {
        public UserId Resolve(TCommand source, RoleGroup destination, UserId destMember,
            ResolutionContext context) => new(Guid.Parse(userIdGetter.UserId));
    }
}