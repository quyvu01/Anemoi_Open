using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.Contract.MasterData.Commands.DistrictCommands.CreateDistrict;
using Anemoi.Contract.MasterData.Commands.DistrictCommands.UpdateDistrict;
using Anemoi.Contract.MasterData.ModelIds;
using Anemoi.MasterData.Domain.Models;
using AutoMapper;

namespace Anemoi.MasterData.Application.Mappings.DistrictMappings;

public sealed class ToDistrictMapping : Profile
{
    public ToDistrictMapping()
    {
        CreateMap<CreateDistrictCommand, District>()
            .ForMember(x => x.Id, opt => opt.MapFrom(_ => new DistrictId(IdGenerator.NextGuid())))
            .ForMember(x => x.Slug, opt => opt.MapFrom(x => x.Name.GenerateSlug()))
            .ForMember(x => x.SearchHint, opt => opt.MapFrom(x => x.Name.GenerateSearchHint()));
        CreateMap<UpdateDistrictCommand, District>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Slug, opt =>
            {
                opt.PreCondition(x => x.Name is { });
                opt.MapFrom(x => x.Name.GenerateSlug());
            })
            .ForMember(x => x.SearchHint, opt =>
            {
                opt.PreCondition(x => x.Name is { });
                opt.MapFrom(x => x.Name.GenerateSearchHint());
            })
            .ForMember(x => x.ProvinceId, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember is { }));
    }
}