using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Queries.RoleGroupQueries.GetRoleGroup;

public sealed record GetRoleGroupQuery(RoleGroupId RoleGroupId) : IQueryOne<RoleGroupResponse>;