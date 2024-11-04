using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.ModelIds;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.RemoveUsers;

public sealed record RemoveUsersCommand(List<UserId> UserIds) : ICommandVoid;