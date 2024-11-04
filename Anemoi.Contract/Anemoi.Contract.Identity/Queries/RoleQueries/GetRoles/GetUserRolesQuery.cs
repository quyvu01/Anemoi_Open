using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Queries.RoleQueries.GetRoles;

public sealed record GetUserRolesQuery : GetManyQuery, IQueryPaged<UserRoleResponse>;