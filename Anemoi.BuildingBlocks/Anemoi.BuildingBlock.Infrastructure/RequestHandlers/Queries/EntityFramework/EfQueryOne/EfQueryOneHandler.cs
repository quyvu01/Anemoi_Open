using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Responses;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Serilog;

namespace Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryOne;

public abstract class EfQueryOneHandler<TModel, TQuery, TResponse>(
    ISqlRepository<TModel> sqlRepository,
    IMapper mapper,
    ILogger logger)
    :
        IQueryHandler<TQuery, OneOf<TResponse, ErrorDetailResponse>>
    where TModel : class
    where TQuery : class, IQueryOne<TResponse>
    where TResponse : class
{
    private readonly IQueryOneFilter<TModel, TResponse> _startQueryFlow = new QueryOneFlow<TModel, TResponse>();
    protected IMapper Mapper { get; } = mapper;
    protected ISqlRepository<TModel> SqlRepository { get; } = sqlRepository;
    protected ILogger Logger { get; } = logger;

    public virtual async Task<OneOf<TResponse, ErrorDetailResponse>> Handle(TQuery request,
        CancellationToken cancellationToken)
    {
        Logger.Information("Query one for {@RequestType}: {@Request}", request?.GetType().Name, request);
        var buildResult = BuildQueryFlow(_startQueryFlow, request);
        switch (buildResult.QuerySpecialActionType)
        {
            case QuerySpecialActionType.UnKnown:
                throw new UnreachableException("Query special type could not be unknown!");
            case QuerySpecialActionType.ToModel:
            {
                var item = await SqlRepository.GetFirstByConditionAsync(buildResult.Filter,
                    db => buildResult.SpecialAction?
                        .Invoke(db.AsNoTracking()) ?? db.AsNoTracking(), cancellationToken);
                if (item is null) return buildResult.ErrorDetail.ToErrorDetailResponse();
                var result = await MapToResultAsync(request, item);
                await AfterQueryAsync(request, result, cancellationToken);
                return result;
            }
            case QuerySpecialActionType.ToTarget:
            default:
            {
                var response = await GetResponseAsync(buildResult, SqlRepository, cancellationToken);
                if (response is null) return buildResult.ErrorDetail.ToErrorDetailResponse();
                var res = await MapToResultAsync(request, response);
                await AfterQueryAsync(request, response, cancellationToken);
                return res;
            }
        }
    }

    private static async Task<TResponse> GetResponseAsync(
        IQueryOneFlowBuilder<TModel, TResponse> queryOneFlowBuilder,
        ISqlRepository<TModel> sqlRepository, CancellationToken cancellationToken)
    {
        var collection = sqlRepository.GetQueryable(queryOneFlowBuilder.Filter)
            .AsNoTracking();
        if (queryOneFlowBuilder.SpecialActionToResponse is null) return null;
        var result = await queryOneFlowBuilder.SpecialActionToResponse.Invoke(collection)
            .FirstOrDefaultAsync(cancellationToken);
        return result;
    }

    protected virtual Task AfterQueryAsync(TQuery query, TResponse response,
        CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task<TResponse> MapToResultAsync(TQuery query, OneOf<TModel, TResponse> modelOrResponse)
        => Task.FromResult(modelOrResponse.Match(i => Mapper.Map<TResponse>(i), rs => rs));

    protected abstract IQueryOneFlowBuilder<TModel, TResponse> BuildQueryFlow(
        IQueryOneFilter<TModel, TResponse> fromFlow, TQuery query);
}