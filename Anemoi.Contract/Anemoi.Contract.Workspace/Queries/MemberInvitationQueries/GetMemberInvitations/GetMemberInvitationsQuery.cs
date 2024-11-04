using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.MemberInvitationQueries.GetMemberInvitations;

public sealed record GetMemberInvitationsQuery : GetManyQuery, IQueryPaged<MemberInvitationResponse>;