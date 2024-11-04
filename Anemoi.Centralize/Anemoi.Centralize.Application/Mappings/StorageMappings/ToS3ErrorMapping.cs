using Anemoi.BuildingBlock.Application.Responses;
using AutoMapper;

namespace Anemoi.Centralize.Application.Mappings.StorageMappings;

public class ToS3ErrorMapping : Profile
{
    public ToS3ErrorMapping()
    {
        CreateMap<Amazon.S3.AmazonS3Exception, ErrorDetailResponse>()
            .ForMember(x => x.Messages, opt => opt.MapFrom(x => new[] { x.Message }))
            .ForMember(x => x.Code, opt => opt.MapFrom(x => x.ErrorCode));
    }
}