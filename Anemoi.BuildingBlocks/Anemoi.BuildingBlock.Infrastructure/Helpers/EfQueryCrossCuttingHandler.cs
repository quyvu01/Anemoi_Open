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
using Newtonsoft.Json;
using Serilog;

namespace Anemoi.BuildingBlock.Infrastructure.Helpers;

public class EfQueryCrossCuttingHandler<TModel, TQuery>(
    ISqlRepository<TModel> sqlRepository,
    IMapper mapper,
    ILogger logger,
    Func<TQuery, Expression<Func<TModel, bool>>> filterFunc,
    Expression<Func<TModel, CrossCuttingDataResponse>> howToGetDataDefault)
    : EfQueryCollectionHandler<TModel, TQuery, CrossCuttingDataResponse>(sqlRepository, mapper, logger)
    where TModel : class
    where TQuery : GetDataMappableQuery
{
    private string _idAlias;

    /// <summary>
    /// Note that the Id will automatically be selected. To modify this one, please update this method either
    /// </summary>
    protected virtual string SetIdAlias() => "Id";

    private static readonly
        Lazy<ConcurrentDictionary<QueryExpressionData, Expression<Func<TModel, CrossCuttingDataResponse>>>>
        lazyStorage = new(() =>
            new ConcurrentDictionary<QueryExpressionData, Expression<Func<TModel, CrossCuttingDataResponse>>>());

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
            .WithSpecialAction(x => x.Select(BuildResponse(query)))
            .WithSortFieldWhenNotSet(_ => Guid.NewGuid())
            .WithSortedDirectionWhenNotSet(SortedDirection.Ascending);

    private Expression<Func<TModel, CrossCuttingDataResponse>> BuildResponse(TQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.Expression)) return howToGetDataDefault;

        return lazyStorage.Value.GetOrAdd(new QueryExpressionData(request.Expression, typeof(TModel)), expressionData =>
        {
            var expression = expressionData.Expression;
            var parameter = Expression.Parameter(typeof(TModel), "x");

            // Access the Id property on the model
            var idProperty = Expression.Property(parameter, _idAlias ?? "Id");
            var toStringMethod = typeof(object).GetMethod(nameof(ToString), Type.EmptyTypes);
            var idAsString = Expression.Call(idProperty, toStringMethod!);

            var expressionParts = expression.Split('.');
            Expression currentExpression = parameter;
            var currentType = typeof(TModel);

            foreach (var part in expressionParts)
            {
                // Handle collection access with an index (e.g., "ApplicationUserRoleGroups[0]")
                var bracketIndex = part.IndexOf('[');
                if (bracketIndex != -1 && part.EndsWith("]"))
                {
                    var collectionPropertyName = part[..bracketIndex];
                    var collectionProperty = currentType.GetProperty(collectionPropertyName);
                    if (collectionProperty == null)
                        throw new ArgumentException(
                            $"Property '{collectionPropertyName}' does not exist on type '{currentType.FullName}'");

                    currentExpression = Expression.Property(currentExpression, collectionProperty);
                    currentExpression = Expression.Call(
                        typeof(Enumerable),
                        nameof(Enumerable.FirstOrDefault),
                        [collectionProperty.PropertyType.GetGenericArguments()[0]],
                        currentExpression
                    );

                    currentType = collectionProperty.PropertyType.GetGenericArguments()[0];
                }
                else if (part == "Count")
                {
                    // Handle "Count" for collections
                    currentExpression = Expression.Property(currentExpression, "Count");
                    currentType = typeof(int);
                }
                else
                {
                    // Handle normal property access
                    var propertyInfo = currentType.GetProperty(part);
                    if (propertyInfo == null)
                        throw new ArgumentException(
                            $"Property '{part}' does not exist on type '{currentType.FullName}'");

                    currentExpression = Expression.Property(currentExpression, propertyInfo);
                    currentType = propertyInfo.PropertyType;
                }
            }

            // Serialize the final value expression using Newtonsoft.Json
            var serializeObjectMethod =
                typeof(JsonConvert).GetMethod(nameof(JsonConvert.SerializeObject), [typeof(object)]);
            var serializeCall = Expression.Call(serializeObjectMethod!,
                Expression.Convert(currentExpression, typeof(object)));

            // Create member bindings for Id and serialized Value
            var bindings = new List<MemberBinding>
            {
                Expression.Bind(typeof(CrossCuttingDataResponse).GetProperty(nameof(CrossCuttingDataResponse.Id))!,
                    idAsString),
                Expression.Bind(typeof(CrossCuttingDataResponse).GetProperty(nameof(CrossCuttingDataResponse.Value))!,
                    serializeCall)
            };

            // Create a new CrossCuttingDataResponse object
            var newExpression = Expression.MemberInit(Expression.New(typeof(CrossCuttingDataResponse)), bindings);

            // Return the lambda expression
            return Expression.Lambda<Func<TModel, CrossCuttingDataResponse>>(newExpression, parameter);
        });
    }
}