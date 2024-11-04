using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Queries.IdentityPolicyQueries.GetIdentityPolicies;

public sealed record GetIdentityPoliciesQuery : IQueryCollection<IdentityPolicyResponse>;