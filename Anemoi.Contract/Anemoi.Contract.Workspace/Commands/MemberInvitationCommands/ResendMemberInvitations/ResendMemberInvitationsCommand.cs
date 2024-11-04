using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Workspace.ModelIds;

namespace Anemoi.Contract.Workspace.Commands.MemberInvitationCommands.ResendMemberInvitations;

public sealed record ResendMemberInvitationsCommand(List<MemberInvitationId> Ids) : ICommandVoid;