using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.CrossCuttingQueries;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using AutoMapper;
using Serilog;

namespace Anemoi.BuildingBlock.Infrastructure.Helpers;

public class EfQueryNameMapIdCrossCuttingHandler<TModel, TQuery>(
    ISqlRepository<TModel> sqlRepository,
    IMapper mapper,
    ILogger logger,
    Func<TQuery, Expression<Func<TModel, bool>>> filterFunc)
    : EfQueryCollectionHandler<TModel, TQuery, CrossCuttingDataResponse>(sqlRepository, mapper, logger)
    where TModel : class
    where TQuery : GetNameMapIdQuery
{
    private static readonly
        Lazy<ConcurrentDictionary<Type, Expression<Func<TModel, CrossCuttingDataResponse>>>>
        lazyStorage = new(() =>
            new ConcurrentDictionary<Type, Expression<Func<TModel, CrossCuttingDataResponse>>>());

    private string _idAlias;
    protected virtual string SetIdAlias() => "Id";

    public override Task<CollectionResponse<CrossCuttingDataResponse>> Handle(TQuery request,
        CancellationToken cancellationToken)
    {
        _idAlias = SetIdAlias();
        return base.Handle(request, cancellationToken);
    }

    protected override IQueryListFlowBuilder<TModel, CrossCuttingDataResponse> BuildQueryFlow(
        IQueryListFilter<TModel, CrossCuttingDataResponse> fromFlow, TQuery query)
        => fromFlow
            .WithFilter(filterFunc.Invoke(query))
            .WithSpecialAction(x => x.Select(BuildResponse()))
            .WithSortFieldWhenNotSet(_ => Guid.NewGuid())
            .WithSortedDirectionWhenNotSet(SortedDirection.Ascending);

    private Expression<Func<TModel, CrossCuttingDataResponse>> BuildResponse()
    {
        return lazyStorage.Value
            // ReSharper disable once HeapView.CanAvoidClosure
            .GetOrAdd(typeof(TModel), _ =>
            {
                // Create a parameter expression (p)
                var parameter = Expression.Parameter(typeof(TModel), "x");
                // Access the property (p.PropertyName)
                var toStringMethod = typeof(object).GetMethod(nameof(ToString), Type.EmptyTypes);
                var idProperty = Expression.Property(parameter, _idAlias ?? "Id");
                var idAsString = Expression.Call(idProperty, toStringMethod!);
                List<MemberBinding> bindings =
                [
                    Expression.Bind(typeof(CrossCuttingDataResponse).GetProperty(nameof(CrossCuttingDataResponse.Id))!,
                        idAsString),
                    Expression.Bind(typeof(CrossCuttingDataResponse)
                        .GetProperty(nameof(CrossCuttingDataResponse.Value))!, parameter),
                ];

                var newExpression = Expression.MemberInit(Expression.New(typeof(CrossCuttingDataResponse)), bindings);
                return Expression.Lambda<Func<TModel, CrossCuttingDataResponse>>(newExpression, parameter);
            });
    }
}