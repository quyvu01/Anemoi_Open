using Anemoi.Contract.Identity.Responses;
using Anemoi.Grpc.Identity;
using AutoMapper;

namespace Anemoi.Centralize.Application.Mappings.IdentityMappings;

public sealed class ToIdentityMapping : Profile
{
    public ToIdentityMapping()
    {
        CreateMap<AuthenticateSucceed, AuthenticationSuccessResponse>()
            .ForMember(x => x.ExpiredIn, opt => opt.MapFrom(x => x.ExpiredIn.ToDateTime()));
    }
}