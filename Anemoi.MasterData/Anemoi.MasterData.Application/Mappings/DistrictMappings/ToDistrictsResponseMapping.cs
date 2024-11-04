using Anemoi.Contract.MasterData.Responses;
using Anemoi.MasterData.Domain.Models;
using AutoMapper;

namespace Anemoi.MasterData.Application.Mappings.DistrictMappings;

public sealed class ToDistrictsResponseMapping : Profile
{
    public ToDistrictsResponseMapping()
    {
        CreateMap<District, DistrictResponse>();
    }
}