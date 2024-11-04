using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.Contract.Workspace.Commands.MemberInvitationCommands.CreateMemberInvitation;
using Anemoi.Contract.Workspace.ModelIds;
using AutoMapper;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Mappings.MemberInvitationMappings;

public sealed class ToMemberInvitationMapping : Profile
{
    public ToMemberInvitationMapping()
    {
        CreateMap<MemberInvitationData, MemberInvitation>()
            .ForMember(x => x.Id, opt => opt.MapFrom(_ => new MemberInvitationId(IdGenerator.NextGuid())))
            .ForMember(x => x.CreatedTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(x => x.CreatorId, opt => opt.MapFrom<UserIdValueResolver>())
            .ForMember(a => a.WorkspaceId, opt => opt.MapFrom<WorkspaceIdValueResolver>())
            .AfterMap((src, des) =>
            {
                des.MemberInvitationMapOrganizations = src.OrganizationIds
                    .Select(a => new MemberInvitationMapOrganization
                    {
                        Id = new MemberInvitationMapOrganizationId(IdGenerator.NextGuid()),
                        MemberInvitationId = des.Id,
                        OrganizationId = a
                    }).ToList();
            });
    }

    private class UserIdValueResolver(IUserIdGetter userIdGetter) :
        IValueResolver<MemberInvitationData, MemberInvitation, string>
    {
        public string Resolve(MemberInvitationData source, MemberInvitation destination, string destMember,
            ResolutionContext context)
            => userIdGetter.UserId;
    }

    private class WorkspaceIdValueResolver(IWorkspaceIdGetter workspaceIdGetter)
        : IValueResolver<MemberInvitationData, MemberInvitation, WorkspaceId>
    {
        public WorkspaceId Resolve(MemberInvitationData source, MemberInvitation destination, WorkspaceId destMember,
            ResolutionContext context) => new(Guid.Parse(workspaceIdGetter.WorkspaceId));
    }
}