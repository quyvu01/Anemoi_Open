using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.MemberQueries.GetMember;

public record GetMemberQuery(MemberId Id) : IQueryOne<MemberResponse>;