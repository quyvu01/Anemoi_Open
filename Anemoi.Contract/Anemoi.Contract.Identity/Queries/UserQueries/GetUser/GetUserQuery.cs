using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Queries.UserQueries.GetUser;

public sealed record GetUserQuery(UserId Id) : IQueryOne<UserResponse>;