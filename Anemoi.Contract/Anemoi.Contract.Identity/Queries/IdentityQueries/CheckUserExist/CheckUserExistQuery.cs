using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Queries.IdentityQueries.CheckUserExist;

public sealed record CheckUserExistQuery(string Email) : IQueryOne<UserWithEmailResponse>;