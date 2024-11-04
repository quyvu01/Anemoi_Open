using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.Contract.Identity.Responses;
using Newtonsoft.Json;

namespace Anemoi.Contract.Identity.Queries.UserQueries.GetUsersByClaims;

public sealed record GetUsersByClaimsQuery(List<string> ClaimTypes,
    [property: JsonIgnore] List<Guid> UserIds) : GetManyQuery, IQueryPaged<UserResponse>;