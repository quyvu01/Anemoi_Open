using Anemoi.Contract.Workspace.Responses;
using AutoMapper;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Mappings.MemberInvitationMappings;

public class ToMemberInvitationResponseMapping : Profile
{
    public ToMemberInvitationResponseMapping()
    {
        CreateMap<MemberInvitation, MemberInvitationResponse>();
    }
}