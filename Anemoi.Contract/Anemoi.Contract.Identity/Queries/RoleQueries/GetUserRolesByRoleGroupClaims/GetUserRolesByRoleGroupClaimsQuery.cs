using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Identity.Contracts;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Queries.RoleQueries.GetUserRolesByRoleGroupClaims;

public sealed record GetUserRolesByRoleGroupClaimsQuery(UserId UserId, List<RoleGroupClaimContract> RoleGroupClaims) :
    IQueryCollection<RoleGroupResponse>;