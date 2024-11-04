using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.ApplicationModels;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.CrossCuttingQueries;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using AutoMapper;
using Serilog;

namespace Anemoi.BuildingBlock.Infrastructure.Helpers;

public class EfQueryCountingCrossCuttingHandler<TModel, TQuery>(
    ISqlRepository<TModel> sqlRepository,
    IMapper mapper,
    ILogger logger) :
    EfQueryCollectionHandler<TModel, TQuery, CrossCuttingDataResponse>(sqlRepository, mapper, logger)
    where TModel : class
    where TQuery : GetDataCountingQuery
{
    private static readonly
        Lazy<ConcurrentDictionary<QueryExpressionData, Expression<Func<TModel, CrossCuttingDataResponse>>>>
        lazyStorage = new(() =>
            new ConcurrentDictionary<QueryExpressionData, Expression<Func<TModel, CrossCuttingDataResponse>>>());

    public override async Task<CollectionResponse<CrossCuttingDataResponse>> Handle(TQuery request,
        CancellationToken cancellationToken)
    {
        var result = await base.Handle(request, cancellationToken);
        var countingGrouped = result
            .Items
            .GroupBy(x => x.Id)
            .Select(a => new CrossCuttingDataResponse
            {
                Id = a.Key, Value = a.Count().ToString()
            });
        return new CollectionResponse<CrossCuttingDataResponse>(countingGrouped.ToList());
    }

    protected override IQueryListFlowBuilder<TModel, CrossCuttingDataResponse> BuildQueryFlow(
        IQueryListFilter<TModel, CrossCuttingDataResponse> fromFlow, TQuery query)
        => fromFlow
            .WithFilter(BuildFilter(query))
            .WithSpecialAction(x => x.Select(BuildResponse(query)))
            .WithSortFieldWhenNotSet(_ => Guid.NewGuid())
            .WithSortedDirectionWhenNotSet(SortedDirection.Ascending);

    private static Expression<Func<TModel, bool>> BuildFilter(TQuery query)
    {
        var parameter = Expression.Parameter(typeof(TModel), "x");
        var property = Expression.Property(parameter, query.Expression);
        var selectorsConstant = Expression.Constant(query.Selectors);
        var containsMethod = typeof(List<string>).GetMethod("Contains", [typeof(string)]);
        var containsCall = Expression.Call(selectorsConstant, containsMethod!, property);
        return Expression.Lambda<Func<TModel, bool>>(containsCall, parameter);
    }

    private static Expression<Func<TModel, CrossCuttingDataResponse>> BuildResponse(TQuery request)
    {
        // Return the default expression if the query's expression is null or empty
        if (string.IsNullOrWhiteSpace(request.Expression))
            throw new ArgumentNullException($"'{nameof(request.Expression)}' is required!");
        return lazyStorage.Value
            // ReSharper disable once HeapView.CanAvoidClosure
            .GetOrAdd(new QueryExpressionData(request.Expression, typeof(TModel)), expressionData =>
            {
                // Create a parameter expression (p)
                var parameter = Expression.Parameter(typeof(TModel), "x");
                // Access the property (p.PropertyName)
                var property = Expression.Property(parameter, expressionData.Expression);
                var toStringMethod = typeof(object).GetMethod(nameof(ToString), Type.EmptyTypes)!;
                var propertyAsString = Expression.Call(property, toStringMethod!);

                var bindings = new List<MemberBinding>
                {
                    Expression.Bind(typeof(CrossCuttingDataResponse).GetProperty(nameof(CrossCuttingDataResponse.Id))!,
                        propertyAsString)
                };

                var newExpression = Expression.MemberInit(Expression.New(typeof(CrossCuttingDataResponse)), bindings);
                return Expression.Lambda<Func<TModel, CrossCuttingDataResponse>>(newExpression, parameter);
            });
    }
}