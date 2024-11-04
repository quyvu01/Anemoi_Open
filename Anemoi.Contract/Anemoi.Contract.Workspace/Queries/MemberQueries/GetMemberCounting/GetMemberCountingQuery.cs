using Anemoi.BuildingBlock.Application.Cqrs.Queries;

namespace Anemoi.Contract.Workspace.Queries.MemberQueries.GetMemberCounting;

public sealed record GetMemberCountingQuery(bool? IsActivated, bool? IsRemoved) : IQueryCounting;