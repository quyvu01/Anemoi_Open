using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Queries.UserQueries.GetUsers;

public sealed record GetUsersQuery(string SearchKey) : GetManyQuery, IQueryPaged<UserResponse>;