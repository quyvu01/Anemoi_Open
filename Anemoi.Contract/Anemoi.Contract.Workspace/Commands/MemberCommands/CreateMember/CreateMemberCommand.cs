using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Responses;
using Newtonsoft.Json;

namespace Anemoi.Contract.Workspace.Commands.MemberCommands.CreateMember;

public sealed record CreateMemberCommand(
    MemberInvitationId MemberInvitationId,
    [property: JsonIgnore] string UserId,
    [property: JsonIgnore] List<string> RoleGroupIds)
    : ICommandResult<MemberIdResponse>;