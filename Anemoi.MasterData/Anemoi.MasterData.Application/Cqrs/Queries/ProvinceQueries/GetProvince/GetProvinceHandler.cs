using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryOne;
using Anemoi.Contract.MasterData.Errors;
using Anemoi.Contract.MasterData.Queries.ProvinceQueries.GetProvince;
using Anemoi.Contract.MasterData.Responses;
using Anemoi.MasterData.Domain.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;

namespace Anemoi.MasterData.Application.Cqrs.Queries.ProvinceQueries.GetProvince;

public sealed class GetProvinceHandler(ISqlRepository<Province> sqlRepository, IMapper mapper, ILogger logger)
    : EfQueryOneHandler<Province, GetProvinceQuery, ProvinceResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryOneFlowBuilder<Province, ProvinceResponse> BuildQueryFlow(
        IQueryOneFilter<Province, ProvinceResponse> fromFlow, GetProvinceQuery query)
        => fromFlow
            .WithFilter(x => x.Id == query.Id)
            .WithSpecialAction(x => x.ProjectTo<ProvinceResponse>(Mapper.ConfigurationProvider))
            .WithErrorIfNull(MasterDataErrorDetail.ProvinceError.NotFound());
}