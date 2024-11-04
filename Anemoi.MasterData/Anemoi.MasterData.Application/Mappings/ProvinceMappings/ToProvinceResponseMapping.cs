using Anemoi.Contract.MasterData.Responses;
using Anemoi.MasterData.Domain.Models;
using AutoMapper;

namespace Anemoi.MasterData.Application.Mappings.ProvinceMappings;

public sealed class ToProvinceResponseMapping : Profile
{
    public ToProvinceResponseMapping()
    {
        CreateMap<Province, ProvinceResponse>();
    }
}