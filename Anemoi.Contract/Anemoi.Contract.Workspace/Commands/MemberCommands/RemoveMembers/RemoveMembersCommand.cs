using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Workspace.ModelIds;

namespace Anemoi.Contract.Workspace.Commands.MemberCommands.RemoveMembers;

public sealed record RemoveMembersCommand(List<MemberId> Ids) : ICommandVoid;