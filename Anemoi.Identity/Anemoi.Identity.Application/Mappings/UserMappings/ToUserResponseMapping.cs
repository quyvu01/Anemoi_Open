using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Domain.Models;
using AutoMapper;

namespace Anemoi.Identity.Application.Mappings.UserMappings;

public sealed class ToUserResponseMapping : Profile
{
    public ToUserResponseMapping()
    {
        CreateMap<User, UserResponse>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.UserId));
        CreateMap<User, UserWithEmailResponse>();
    }
}