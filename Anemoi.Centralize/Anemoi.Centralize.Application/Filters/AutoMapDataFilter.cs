using System.Diagnostics;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.CrossCuttingQueries;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Centralize.Application.ContractAssemblies;
using Anemoi.Centralize.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Anemoi.Centralize.Application.Filters;

public class AutoMapDataFilter(ISender sender, ILogger logger) : IAsyncResultFilter
{
    private static readonly List<Type> mappingTypes = [typeof(IDataMappableOf<>), typeof(IDataCountingOf<>)];

    private static readonly Lazy<Dictionary<Type, Type>> AttributeQueryLazyStorage = new(() =>
    {
        var allContractAssemblies = ContractAssembly.GetAllContractAssemblies();
        return allContractAssemblies.SelectMany(x => x.ExportedTypes)
            .Where(x => (typeof(GetDataMappableQuery).IsAssignableFrom(x) ||
                         typeof(GetDataCountingQuery).IsAssignableFrom(x)) && !x.IsInterface && !x.IsAbstract)
            .Select(queryType =>
            {
                var implementationTypes = queryType.GetInterfaces();
                var requestInterfaceType = implementationTypes.FirstOrDefault(i =>
                    i.IsGenericType && mappingTypes.Contains(i.GetGenericTypeDefinition()));
                if (requestInterfaceType is not { GenericTypeArguments.Length: 1 })
                    throw new NotImplementMapDataException();
                return (CrossCuttingConcernType: requestInterfaceType.GenericTypeArguments.First(),
                    QueryType: queryType);
            }).ToDictionary(k => k.CrossCuttingConcernType, v => v.QueryType);
    });

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is not OkObjectResult okObjectResult)
        {
            await next();
            return;
        }

        try
        {
            await MapDataAsync(okObjectResult.Value);
        }
        catch (Exception e)
        {
            logger.Error("Error while mapping data: {@Error}", e.Message);
        }

        await next();
    }

    private async Task MapDataAsync(object okObjectResult)
    {
        var stw = Stopwatch.StartNew();
        var allPropertyDatas = ReflectionHelpers.GetCrossCuttingProperties(okObjectResult).ToList();
        logger.Information("[AutoMapDataFilter] Get all properties using Reflection spent: {@TimeSpent}", stw.Elapsed);
        stw.Restart();
        var crossCuttingTypeWithIds = ReflectionHelpers
            .GetCrossCuttingTypeWithIds(allPropertyDatas, AttributeQueryLazyStorage.Value.Keys);
        var orderedCrossCuttings = crossCuttingTypeWithIds
            .GroupBy(a => a.Order)
            .OrderBy(a => a.Key);
        foreach (var orderedCrossCutting in orderedCrossCuttings)
        {
            var orderedPropertyDatas = allPropertyDatas
                .Where(x => x.Order == orderedCrossCutting.Key);
            var tasks = orderedCrossCutting
                .Select(async x =>
                {
                    var emptyCollection = new CollectionResponse<CrossCuttingDataResponse>([]);
                    var propertyCalledStorages = x.PropertyCalledLaters.ToList();
                    if (propertyCalledStorages is not { Count: > 0 })
                        return (x.CrossCuttingType, x.Expression, Response: emptyCollection);
                    if (!AttributeQueryLazyStorage.Value.TryGetValue(x.CrossCuttingType, out var queryType))
                        return (x.CrossCuttingType, x.Expression, Response: emptyCollection);
                    try
                    {
                        var selectors = propertyCalledStorages
                            .Select(c => c.Func.DynamicInvoke(c.Model)?.ToString());
                        Func<object> selectorsByTypeFunc = (typeof(GetDataMappableQuery).IsAssignableFrom(queryType),
                                typeof(GetDataCountingQuery).IsAssignableFrom(queryType)) switch
                            {
                                (true, _) => () =>
                                {
                                    var selectorIds = selectors.Where(c => Guid.TryParse(c, out _))
                                        .Select(Guid.Parse)
                                        .Distinct()
                                        .ToList();
                                    return selectorIds is { Count: > 0 } ? selectorIds : null;
                                },
                                _ => () =>
                                {
                                    var selectorValues = selectors.Where(a => a is not null).Distinct().ToList();
                                    return selectorValues is { Count: > 0 } ? selectorValues : null;
                                }
                            };
                        var selectorsByType = selectorsByTypeFunc();
                        if (selectorsByType is null)
                            return (x.CrossCuttingType, x.Expression, Response: emptyCollection);
                        var cancellationTokenSource = CancellationTokenSource
                            .CreateLinkedTokenSource(CancellationToken.None);
                        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(2));
                        var query = Activator.CreateInstance(queryType!, selectorsByType, x.Expression);
                        if (query is null) return (x.CrossCuttingType, x.Expression, Response: emptyCollection);
                        var result = await sender.Send(query, cancellationTokenSource.Token);
                        var response = result as CollectionResponse<CrossCuttingDataResponse> ?? emptyCollection;
                        return (x.CrossCuttingType, x.Expression, Response: response);
                    }
                    catch (Exception)
                    {
                        Log.Error("[AutoMapDataFilter] Error while getting Data for queryType: {@QueryType}",
                            queryType!.Name);
                        return (x.CrossCuttingType, x.Expression, Response: emptyCollection);
                    }
                });
            var orderedTasks = await Task.WhenAll(tasks);
            ReflectionHelpers.MapResponseData(orderedPropertyDatas, orderedTasks.ToList());
        }

        logger.Information("[AutoMapDataFilter] Map all properties using Reflection spent: {@TimeSpent}", stw.Elapsed);
    }
}