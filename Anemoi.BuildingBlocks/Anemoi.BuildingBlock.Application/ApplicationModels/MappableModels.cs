using System;
using System.Collections.Generic;
using System.Reflection;
using Anemoi.BuildingBlock.Application.Abstractions;

namespace Anemoi.BuildingBlock.Application.ApplicationModels;

public sealed record CrossCuttingDataProperty(
    PropertyInfo PropertyInfo,
    object Model,
    ICrossCuttingConcernCore Attribute,
    Delegate Func,
    string Expression,
    int Order);

public sealed record CrossCuttingDataPropertyCache(
    ICrossCuttingConcernCore Attribute,
    Delegate Func,
    string Expression,
    int Order);


public sealed record CrossCuttingTypeData(
    Type CrossCuttingType,
    IEnumerable<PropertyCalledLater> PropertyCalledLaters,
    string Expression,
    int Order);


public sealed record PropertyCalledLater(object Model, Delegate Func);