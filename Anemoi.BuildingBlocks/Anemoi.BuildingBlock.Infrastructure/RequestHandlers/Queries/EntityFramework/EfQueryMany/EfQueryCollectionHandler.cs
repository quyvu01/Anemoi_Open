using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Responses;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Serilog;

namespace Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;

public abstract class EfQueryCollectionHandler<TModel, TQuery, TResponse>(
    ISqlRepository<TModel> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : IQueryHandler<TQuery, CollectionResponse<TResponse>>
    where TModel : class
    where TQuery : IQueryCollection<TResponse>
    where TResponse : class
{
    protected IMapper Mapper { get; } = mapper;
    protected ISqlRepository<TModel> SqlRepository { get; } = sqlRepository;
    protected ILogger Logger { get; } = logger;
    private readonly IQueryListFilter<TModel, TResponse> _queryFlow = new QueryManyFlow<TModel, TResponse>();

    public virtual async Task<CollectionResponse<TResponse>> Handle(TQuery request, CancellationToken cancellationToken)
    {
        Logger.Information("Get many query for {@RequestType}: {@Request}", request.GetType().Name, request);
        var buildResult = BuildQueryFlow(_queryFlow, request);
        switch (buildResult.QuerySpecialActionType)
        {
            case QuerySpecialActionType.ToModel:
            {
                var items = await SqlRepository
                    .GetManyByConditionAsync(buildResult.Filter, db =>
                    {
                        var finalFilter = buildResult.SpecialActionToModel?.Invoke(db) ?? db;
                        return finalFilter
                            .AsNoTracking()
                            .OrderByWithDynamic(null, buildResult.SortFieldNameWhenRequestEmpty,
                                buildResult.SortedDirectionWhenRequestEmpty);
                    }, cancellationToken);
                var result = await MapToResultAsync(request, items);
                await AfterQueryAsync(request, result, cancellationToken);
                return new CollectionResponse<TResponse>(result);
            }
            case QuerySpecialActionType.ToTarget:
                var srcQueryable = SqlRepository
                    .GetQueryable(buildResult.Filter)
                    .AsNoTracking();
                var queryable = srcQueryable
                    .OrderByWithDynamic(null, buildResult.SortFieldNameWhenRequestEmpty,
                        buildResult.SortedDirectionWhenRequestEmpty);
                var response = await buildResult.SpecialActionToResponse.Invoke(queryable)
                    .ToListAsync(cancellationToken);
                await AfterQueryAsync(request, response, cancellationToken);
                var res = await MapToResultAsync(request, response);
                return new CollectionResponse<TResponse>(res);
            case QuerySpecialActionType.UnKnown:
            default:
                throw new UnreachableException("Query special type could not be unknown!");
        }
    }

    protected abstract IQueryListFlowBuilder<TModel, TResponse> BuildQueryFlow(
        IQueryListFilter<TModel, TResponse> fromFlow, TQuery query);

    protected virtual Task AfterQueryAsync(TQuery query, List<TResponse> response,
        CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task<List<TResponse>> MapToResultAsync(TQuery query,
        OneOf<List<TModel>, List<TResponse>> modelsOrResponses)
        => modelsOrResponses.Match(m => Task.FromResult(Mapper.Map<List<TResponse>>(m)), Task.FromResult);
}