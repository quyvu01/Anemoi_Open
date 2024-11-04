using System.Linq.Expressions;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.MasterData.Queries.DistrictQueries.GetDistricts;
using Anemoi.Contract.MasterData.Responses;
using Anemoi.MasterData.Domain.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;

namespace Anemoi.MasterData.Application.Cqrs.Queries.DistrictQueries.GetDistricts;

public sealed class GetDistrictsHandler(ISqlRepository<District> sqlRepository, IMapper mapper, ILogger logger)
    : EfQueryPaginationHandler<District, GetDistrictsQuery, DistrictResponse>(sqlRepository, mapper, logger)
{
    protected override IQueryListFlowBuilder<District, DistrictResponse> BuildQueryFlow(
        IQueryListFilter<District, DistrictResponse> fromFlow, GetDistrictsQuery query)
    {
        Expression<Func<District, bool>> provinceIdFiler = query.ProvinceId switch
        {
            { } val => d => d.ProvinceId == val,
            _ => _ => true
        };
        Expression<Func<District, bool>> searchKeyFilter = query.SearchKey switch
        {
            { } val => d => d.SearchHint.Contains(val.GenerateSearchHint()),
            _ => _ => true
        };
        var finalFilter = ExpressionHelper.CombineAnd(provinceIdFiler, searchKeyFilter);
        return fromFlow
            .WithFilter(finalFilter)
            .WithSpecialAction(x => x.ProjectTo<DistrictResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(x => x.Name)
            .WithSortedDirectionWhenNotSet(SortedDirection.Ascending);
    }
}