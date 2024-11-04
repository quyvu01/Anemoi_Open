using System;
using System.Collections.Generic;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.CrossCuttingQueries;
using MassTransit;

namespace Anemoi.BuildingBlock.Application.Abstractions;

[ExcludeFromTopology]
public record DataCountingOf<TAttribute>(List<string> Selectors, string Expression)
    : GetDataCountingQuery(Selectors, Expression), IDataCountingOf<TAttribute>
    where TAttribute : Attribute, IDataCountingCore;