using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.MemberQueries.GetMembersByIds;

public sealed record GetMembersByIdsQuery(List<MemberId> Ids) : IQueryCollection<MemberResponse>;