using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.ApplicationModels;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using Newtonsoft.Json;
using Serilog;

namespace Anemoi.BuildingBlock.Application.Helpers;

public static class ReflectionHelpers
{
    private static readonly ConcurrentDictionary<PropertyInfo, CrossCuttingDataPropertyCache>
        CrossCuttingPropertiesCache = new();

    public static IEnumerable<CrossCuttingDataProperty> GetCrossCuttingProperties(object rootObject)
    {
        Log.Information("CrossCuttingPropertiesCache Stored: {@NumOfPropertyInfo} propertyInfo(s)",
            CrossCuttingPropertiesCache.Count);
        if (rootObject is None) yield break;
        var stack = new Stack<object>();
        var rootType = rootObject.GetType();
        if (typeof(IEnumerable).IsAssignableFrom(rootType) && rootType != typeof(string))
        {
            EnumerableObject(rootObject, stack);
            goto startWithStack;
        }

        stack.Push(rootObject);
        startWithStack:
        while (stack.Count > 0)
        {
            var currentObject = stack.Pop();
            if (currentObject == null) continue;
            var currentType = currentObject.GetType();
            var properties = currentType.GetProperties();
            foreach (var property in properties)
            {
                if (CrossCuttingPropertiesCache.TryGetValue(property, out var propertyCache))
                {
                    var func = propertyCache.Func;
                    yield return new CrossCuttingDataProperty(property, currentObject, propertyCache.Attribute, func,
                        propertyCache.Expression, propertyCache.Order);
                    continue;
                }

                var crossCutting = property.GetCustomAttributes(true)
                    .OfType<ICrossCuttingConcernCore>()
                    .FirstOrDefault();
                if (crossCutting is not null && Attribute.IsDefined(property, crossCutting.GetType()))
                {
                    var paramExpression = Expression.Parameter(currentObject.GetType());
                    var propExpression = Expression.Property(paramExpression, crossCutting.PropertyName);
                    var expression = Expression.Lambda(propExpression, paramExpression);
                    var func = expression.Compile();
                    yield return new CrossCuttingDataProperty(property, currentObject, crossCutting, func,
                        crossCutting.Expression, crossCutting.Order);
                    CrossCuttingPropertiesCache.TryAdd(property,
                        new CrossCuttingDataPropertyCache(crossCutting, func, crossCutting.Expression,
                            crossCutting.Order));
                    continue;
                }

                try
                {
                    var propertyValue = property.GetValue(currentObject);
                    if (propertyValue == null) continue;
                    if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) &&
                        property.PropertyType != typeof(string))
                    {
                        EnumerableObject(propertyValue, stack);
                        continue;
                    }

                    if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                        stack.Push(propertyValue);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }

    private static void EnumerableObject(object propertyValue, Stack<object> stack)
    {
        foreach (var item in (IEnumerable)propertyValue)
            if (item != null && !stack.Contains(item) && item.GetType() != typeof(string))
                stack.Push(item);
    }

    public static IEnumerable<CrossCuttingTypeData> GetCrossCuttingTypeWithIds(
        IEnumerable<CrossCuttingDataProperty> datas, IEnumerable<Type> crossCuttingTypes) =>
        datas
            .GroupBy(x => new { AttributeType = x.Attribute.GetType(), x.Expression, x.Order })
            .Join(crossCuttingTypes, d => d.Key.AttributeType, x => x,
                (d, x) => new CrossCuttingTypeData(x, d
                        .Select(a => new PropertyCalledLater(a.Model, a.Func)), d.Key.Expression,
                    d.Key.Order));

    public static void MapResponseData(IEnumerable<CrossCuttingDataProperty> allPropertyDatas,
        List<(Type CrossCuttingType, string Expression, CollectionResponse<CrossCuttingDataResponse> Response)>
            dataTasks)
    {
        allPropertyDatas.Join(dataTasks, ap => (ap.Attribute.GetType(), ap.Expression),
            dt => (dt.CrossCuttingType, dt.Expression),
            (ap, dt) =>
            {
                var value = dt.Response.Items?
                    .FirstOrDefault(a => a.Id == ap.Func.DynamicInvoke(ap.Model)?.ToString())?.Value;
                if (value is null || ap.PropertyInfo is null) return value;

                try
                {
                    var valueSet = JsonConvert.DeserializeObject(value, ap.PropertyInfo.PropertyType);
                    ap.PropertyInfo.SetValue(ap.Model, valueSet);
                }
                catch (Exception)
                {
                    try
                    {
                        if (ap.PropertyInfo.PropertyType == typeof(string)) ap.PropertyInfo.SetValue(ap.Model, value);
                    }
                    catch
                    {
                        Log.Error("Error while mapping propertyType: {@PropertyType}: with value: {@Value}",
                            ap.PropertyInfo.PropertyType, value);
                    }
                }

                return value;
            }).IteratorVoid();
    }
}