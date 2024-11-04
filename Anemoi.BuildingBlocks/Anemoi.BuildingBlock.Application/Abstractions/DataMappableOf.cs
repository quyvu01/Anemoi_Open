using System;
using System.Collections.Generic;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.CrossCuttingQueries;
using MassTransit;

namespace Anemoi.BuildingBlock.Application.Abstractions;

[ExcludeFromTopology]
public record DataMappableOf<TAttribute>(List<Guid> SelectorIds, string Expression)
    : GetDataMappableQuery(SelectorIds, Expression), IDataMappableOf<TAttribute>
    where TAttribute : Attribute, IDataMappableCore;