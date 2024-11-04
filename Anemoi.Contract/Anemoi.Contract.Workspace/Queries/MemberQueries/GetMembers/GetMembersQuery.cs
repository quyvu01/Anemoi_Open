using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.MemberQueries.GetMembers;

public sealed record GetMembersQuery(List<string> UserIds, bool? IsActivated, bool? IsRemoved)
    : GetManyQuery, IQueryPaged<MemberResponse>;