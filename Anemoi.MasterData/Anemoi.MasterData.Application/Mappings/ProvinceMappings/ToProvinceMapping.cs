using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.Contract.MasterData.Commands.ProvinceCommands.CreateProvince;
using Anemoi.Contract.MasterData.Commands.ProvinceCommands.UpdateProvince;
using Anemoi.Contract.MasterData.ModelIds;
using Anemoi.MasterData.Domain.Models;
using AutoMapper;

namespace Anemoi.MasterData.Application.Mappings.ProvinceMappings;

public sealed class ToProvinceMapping : Profile
{
    public ToProvinceMapping()
    {
        CreateMap<CreateProvinceCommand, Province>()
            .ForMember(x => x.Id, opt => opt.MapFrom(_ => new ProvinceId(IdGenerator.NextGuid())))
            .ForMember(x => x.Slug, opt => opt.MapFrom(x => x.Name.GenerateSlug()))
            .ForMember(x => x.SearchHint, opt => opt.MapFrom(x => x.Name.GenerateSearchHint()));
        CreateMap<UpdateProvinceCommand, Province>()
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
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember is { }));
    }
}