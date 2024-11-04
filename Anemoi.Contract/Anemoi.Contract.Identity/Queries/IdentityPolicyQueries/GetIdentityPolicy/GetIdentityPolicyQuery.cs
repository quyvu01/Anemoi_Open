using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Queries.IdentityPolicyQueries.GetIdentityPolicy;

public sealed record GetIdentityPolicyQuery : IQueryOne<IdentityPolicyResponse>;