using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Grpc.Identity;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Anemoi.Identity.Application.Mappings.AuthenticationMappings;

public sealed class ToAuthenticationSucceedMapping : Profile
{
    public ToAuthenticationSucceedMapping()
    {
        CreateMap<AuthenticationSuccessResponse, AuthenticateSucceed>()
            .ForMember(x => x.ExpiredIn, opt => opt.MapFrom(x => Timestamp.FromDateTime(x.ExpiredIn)));
        CreateMap<ErrorDetailResponse, ErrorDetailResult>()
            .ForMember(x => x.Messages, opt => opt.Ignore())
            .AfterMap((src, des) => des.Messages.AddRange(src.Messages));
        
        CreateMap<RefreshToken, AuthenticationSuccessResponse>()
            .ForMember(x => x.RefreshToken, opt => opt.MapFrom(x => x.Id))
            .ForMember(x => x.Token, opt => opt.MapFrom(x => x.UserToken))
            .ForMember(x => x.ExpiredIn, opt => opt.MapFrom(x => x.TokenExpiryTime));
    }
}