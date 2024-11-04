using Anemoi.Contract.Workspace.Responses;
using AutoMapper;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Mappings.MemberMappings;

public class ToMemberResponseMapping : Profile
{
    public ToMemberResponseMapping()
    {
        CreateMap<Member, MemberResponse>();
        CreateMap<Member, MemberIdResponse>();
    }
}