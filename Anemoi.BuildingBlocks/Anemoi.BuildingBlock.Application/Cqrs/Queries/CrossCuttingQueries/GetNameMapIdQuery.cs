using System.Collections.Generic;
using Anemoi.BuildingBlock.Application.Responses;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.CrossCuttingQueries;

public record GetNameMapIdQuery(List<string> Names) : IQueryCollection<CrossCuttingDataResponse>;