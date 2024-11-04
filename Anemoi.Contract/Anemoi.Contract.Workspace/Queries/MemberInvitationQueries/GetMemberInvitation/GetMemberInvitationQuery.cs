using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.MemberInvitationQueries.GetMemberInvitation;

public sealed record GetMemberInvitationQuery(MemberInvitationId Id) : IQueryOne<MemberInvitationResponse>;