using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.Contract.CrossCuttingConcern.Attributes;

namespace Anemoi.Contract.Identity.Queries.UserQueries.GetCrossCuttingUsers;

public sealed record GetCrossCuttingUsersQuery(List<Guid> SelectorIds, string Expression)
    : DataMappableOf<UserOfAttribute>(SelectorIds, Expression);