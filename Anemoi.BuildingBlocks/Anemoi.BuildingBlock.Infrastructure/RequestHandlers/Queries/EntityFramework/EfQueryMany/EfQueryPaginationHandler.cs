using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Application.Responses;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Serilog;

namespace Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;

public abstract class EfQueryPaginationHandler<TModel, TQuery, TResponse>(
    ISqlRepository<TModel> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : IQueryHandler<TQuery, PaginationResponse<TResponse>>
    where TModel : class
    where TQuery : GetManyQuery, IQueryPaged<TResponse>
    where TResponse : class
{
    protected IMapper Mapper { get; } = mapper;
    protected ISqlRepository<TModel> SqlRepository { get; } = sqlRepository;
    protected ILogger Logger { get; } = logger;
    private readonly IQueryListFilter<TModel, TResponse> _queryFlow = new QueryManyFlow<TModel, TResponse>();

    public virtual async Task<PaginationResponse<TResponse>> Handle(TQuery request,
        CancellationToken cancellationToken)
    {
        Logger.Information("Get many query for {@RequestType}: {@Request}", request.GetType().Name, request);
        var buildResult = BuildQueryFlow(_queryFlow, request);
        switch (buildResult.QuerySpecialActionType)
        {
            case QuerySpecialActionType.ToModel:
            {
                var toModelSrcQueryable = SqlRepository
                    .GetQueryable(buildResult.Filter)
                    .AsNoTracking();
                var toModelQueryable = toModelSrcQueryable
                    .OrderByWithDynamic(request.SortedFieldName, buildResult.SortFieldNameWhenRequestEmpty,
                        request.SortedDirection ?? buildResult.SortedDirectionWhenRequestEmpty);
                var toModelFinalQueryable = buildResult.SpecialActionToModel.Invoke(toModelQueryable);
                var toModelResponse = await toModelFinalQueryable
                    .Offset(request.GetSkip())
                    .Limit(request.GetTake())
                    .ToListAsync(cancellationToken);
                var toModelTotalRecord = await toModelFinalQueryable.LongCountAsync(cancellationToken);
                var result = await MapToResultAsync(request, toModelResponse,
                    toModelTotalRecord);
                await AfterQueryAsync(request, result.Items, cancellationToken);
                return result;
            }
            case QuerySpecialActionType.ToTarget:
                var srcQueryable = SqlRepository
                    .GetQueryable(buildResult.Filter)
                    .AsNoTracking();

                var queryable = srcQueryable
                    .OrderByWithDynamic(request.SortedFieldName, buildResult.SortFieldNameWhenRequestEmpty,
                        request.SortedDirection ?? buildResult.SortedDirectionWhenRequestEmpty);
                var finalQueryable = buildResult.SpecialActionToResponse.Invoke(queryable);
                var response = await finalQueryable
                    .Offset(request.GetSkip())
                    .Limit(request.GetTake())
                    .ToListAsync(cancellationToken);
                var totalRecord = await finalQueryable.LongCountAsync(cancellationToken);
                var res = await MapToResultAsync(request, response, totalRecord);
                await AfterQueryAsync(request, response, cancellationToken);
                return res;
            case QuerySpecialActionType.UnKnown:
            default:
                throw new UnreachableException("Query special type could not be unknown!");
        }
    }

    protected abstract IQueryListFlowBuilder<TModel, TResponse> BuildQueryFlow(
        IQueryListFilter<TModel, TResponse> fromFlow, TQuery query);

    protected virtual Task AfterQueryAsync(TQuery query, List<TResponse> response,
        CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task<PaginationResponse<TResponse>> MapToResultAsync(TQuery query,
        OneOf<List<TModel>, List<TResponse>> modelsOrResponses, long totalRecord) =>
        Task.FromResult(new PaginationResponse<TResponse>(
            modelsOrResponses.Match(i => Mapper.Map<List<TResponse>>(i), rs => rs),
            totalRecord));
}