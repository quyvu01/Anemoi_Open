using Anemoi.Identity.Application.IdentityResults;
using AutoMapper;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.Mappings.RefreshTokenMappings;

public sealed class ToRefreshTokenMapping : Profile
{
    public ToRefreshTokenMapping()
    {
        CreateMap<IdentitySuccess, RefreshToken>()
            .ForMember(x => x.TokenExpiryTime, opt => opt.MapFrom(x => x.TokenExpiryTime))
            .ForMember(x => x.ExpiryDate, opt => opt.MapFrom(x => x.RefreshTokenExpiryTime));
    }
}