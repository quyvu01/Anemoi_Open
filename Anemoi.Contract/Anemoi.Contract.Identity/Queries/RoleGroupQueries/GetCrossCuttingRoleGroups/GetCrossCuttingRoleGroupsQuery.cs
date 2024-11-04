using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.Contract.CrossCuttingConcern.Attributes;

namespace Anemoi.Contract.Identity.Queries.RoleGroupQueries.GetCrossCuttingRoleGroups;

public record GetCrossCuttingRoleGroupsQuery(List<Guid> SelectorIds, string Expression)
    : DataMappableOf<RoleGroupOfAttribute>(SelectorIds, Expression);