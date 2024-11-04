using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Workspace.ModelIds;

namespace Anemoi.Contract.Workspace.Commands.MemberInvitationCommands.CreateMemberInvitation;

public sealed record CreateMemberInvitationsCommand(List<MemberInvitationData> Users) : ICommandVoid;

public sealed record MemberInvitationData
{
    public string Email { get; set; }
    public List<string> RoleGroupIds { get; set; }
    public List<OrganizationId> OrganizationIds { get; set; }
}