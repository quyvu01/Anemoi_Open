using System;
using System.Collections.Generic;
using Anemoi.BuildingBlock.Application.Responses;
using MassTransit;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.CrossCuttingQueries;

[ExcludeFromTopology]
public record GetDataMappableQuery(List<Guid> SelectorIds, string Expression) : IQueryCollection<CrossCuttingDataResponse>;