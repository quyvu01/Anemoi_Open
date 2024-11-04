using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Queries.UserQueries.GetUserWithEmailsByEmails;

public sealed record GetUserWithEmailsByEmailsQuery(List<string> Emails) : IQueryCollection<UserResponse>;