using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Workspace.ModelIds;

namespace Anemoi.Contract.Workspace.Commands.MemberCommands.UpdateActivatedMembers;

public sealed record UpdateActivatedMembersCommand(List<MemberId> Ids, bool IsActivated) : ICommandVoid;

    
