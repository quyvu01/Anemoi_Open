using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.CrossCuttingQueries;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Responses;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Anemoi.BuildingBlock.Infrastructure.Services;

public sealed class DataMappableService(
    IServiceProvider serviceProvider,
    IEnumerable<Assembly> contractAssemblies) : IDataMappableService
{
    private static readonly List<Type> mappingTypes = [typeof(IDataMappableOf<>), typeof(IDataCountingOf<>)];

    private static readonly Lazy<ConcurrentDictionary<Type, MethodInfo>> MethodInfoStorage =
        new(() => new ConcurrentDictionary<Type, MethodInfo>());

    private const string RequestMethod = "GetResponse";

    private readonly Lazy<Dictionary<Type, Type>> AttributeQueryLazyStorage = new(() =>
    {
        return contractAssemblies.SelectMany(x => x.ExportedTypes)
            .Where(x => (typeof(GetDataMappableQuery).IsAssignableFrom(x) ||
                         typeof(GetDataCountingQuery).IsAssignableFrom(x)) && !x.IsInterface && !x.IsAbstract)
            .Select(queryType =>
            {
                var implementationTypes = queryType.GetInterfaces();
                var requestInterfaceType = implementationTypes.FirstOrDefault(i =>
                    i.IsGenericType && mappingTypes.Contains(i.GetGenericTypeDefinition()));
                if (requestInterfaceType is not { GenericTypeArguments.Length: 1 })
                    throw new UnreachableException();
                return (CrossCuttingConcernType: requestInterfaceType.GenericTypeArguments.First(),
                    QueryType: queryType);
            }).ToDictionary(k => k.CrossCuttingConcernType, v => v.QueryType);
    });

    public async Task MapDataAsync(object value, CancellationToken token = default)
    {
        var stw = Stopwatch.StartNew();
        var allPropertyDatas = ReflectionHelpers.GetCrossCuttingProperties(value).ToList();
        Log.Information("[DataMappableService] Get all properties using Reflection spent: {@TimeSpent}", stw.Elapsed);
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

            var tasks = orderedCrossCutting.Select(async x =>
            {
                var emptyCollection = new CollectionResponse<CrossCuttingDataResponse>([]);
                var emptyResponse = (x.CrossCuttingType, x.Expression, Response: emptyCollection);
                var propertyCalledStorages = x.PropertyCalledLaters.ToList();
                if (propertyCalledStorages is not { Count: > 0 }) return emptyResponse;
                if (!AttributeQueryLazyStorage.Value.TryGetValue(x.CrossCuttingType, out var queryType))
                    return emptyResponse;

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
                var query = Activator.CreateInstance(queryType!, selectorsByType, x.Expression);
                if (query is null) return emptyResponse;
                var requestClient = serviceProvider
                    .GetRequiredService(typeof(IRequestClient<>).MakeGenericType(queryType));
                var genericMethod = MethodInfoStorage.Value.GetOrAdd(queryType, _ =>
                {
                    var getResponseMethod = requestClient.GetType().GetMethods()
                        .FirstOrDefault(m =>
                            m.Name == RequestMethod && m.GetParameters() is { Length: 3 } parameters &&
                            parameters[0].ParameterType == queryType &&
                            parameters[1].ParameterType == typeof(CancellationToken) &&
                            parameters[2].ParameterType == typeof(RequestTimeout));
                    if (getResponseMethod is null) return null;
                    // Construct a MethodInfo object for the generic method
                    Type[] responseType = [typeof(CollectionResponse<CrossCuttingDataResponse>)];
                    return getResponseMethod.MakeGenericMethod(responseType);
                });

                try
                {
                    object[] arguments = [query, token, RequestTimeout.Default];
                    // Invoke the method and get the result
                    var requestTask = ((Task<Response<CollectionResponse<CrossCuttingDataResponse>>>)genericMethod
                        .Invoke(requestClient, arguments))!;
                    var response = await requestTask;
                    var result = response.Message;
                    return (x.CrossCuttingType, x.Expression, Response: result);
                }
                catch (Exception e)
                {
                    Log.Error("[DataMappableService] [Error while retrieving cross cutting data]: {@Error}", e.Message);
                    return emptyResponse;
                }
            });
            var orderedTasks = await Task.WhenAll(tasks);
            ReflectionHelpers.MapResponseData(orderedPropertyDatas, orderedTasks.ToList());
        }

        Log.Information("[DataMappableService] Map all properties using Reflection spent: {@TimeSpent}", stw.Elapsed);
    }
}