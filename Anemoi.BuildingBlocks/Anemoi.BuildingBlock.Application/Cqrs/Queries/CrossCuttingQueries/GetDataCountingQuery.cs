using System.Collections.Generic;
using Anemoi.BuildingBlock.Application.Responses;
using MassTransit;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.CrossCuttingQueries;

[ExcludeFromTopology]
public record GetDataCountingQuery(List<string> Selectors, string Expression)
    : IQueryCollection<CrossCuttingDataResponse>;