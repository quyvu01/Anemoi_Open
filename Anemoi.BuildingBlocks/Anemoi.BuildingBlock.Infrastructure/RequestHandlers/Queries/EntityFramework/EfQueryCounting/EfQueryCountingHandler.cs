using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.CountingFlow;
using Anemoi.BuildingBlock.Application.Responses;
using Serilog;

namespace Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryCounting;

public abstract class EfQueryCountingHandler<TModel, TQuery>(ISqlRepository<TModel> sqlRepository, ILogger logger) :
    IQueryHandler<TQuery, CountingResponse>
    where TModel : class
    where TQuery : class, IQueryCounting
{
    private readonly ICountingFilter<TModel> _startQueryFlow = new CountingFlow<TModel>();

    public virtual async Task<CountingResponse> Handle(TQuery request, CancellationToken cancellationToken)
    {
        logger.Information("Counting for {@RequestType}: {@Request}", request?.GetType().Name, request);
        var flowBuilder = BuildQueryFlow(_startQueryFlow, request);
        var count = await sqlRepository
            .CountByConditionAsync(flowBuilder.Filter, null, cancellationToken); // Todo: Update later with filter
        return new CountingResponse { Count = count };
    }

    protected abstract ICountingFlowBuilder<TModel> BuildQueryFlow(
        ICountingFilter<TModel> fromFlow, TQuery query);
}