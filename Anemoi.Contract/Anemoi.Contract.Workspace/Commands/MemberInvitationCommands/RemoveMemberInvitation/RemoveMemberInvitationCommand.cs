using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Workspace.ModelIds;

namespace Anemoi.Contract.Workspace.Commands.MemberInvitationCommands.RemoveMemberInvitation;

public sealed record RemoveMemberInvitationCommand(MemberInvitationId Id) : ICommandVoid;