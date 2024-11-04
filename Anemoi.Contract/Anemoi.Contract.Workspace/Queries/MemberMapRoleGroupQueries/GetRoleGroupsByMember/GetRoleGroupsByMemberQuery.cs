using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.MemberMapRoleGroupQueries.GetRoleGroupsByMember;

public sealed record GetRoleGroupsByMemberQuery(string UserId, WorkspaceId WorkspaceId)
    : IQueryCollection<MemberMapRoleGroupResponse>;