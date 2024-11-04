using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Queries.RoleGroupQueries.GetRoleGroups;

public sealed record GetRoleGroupsQuery(
    string SearchKey,
    List<string> CreatorIds,
    bool? IsDefault,
    List<string> RoleGroupClaimKeys)
    : GetManyQuery, IQueryPaged<RoleGroupsResponse>;