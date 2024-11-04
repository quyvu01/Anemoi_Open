using Anemoi.BuildingBlock.Application.Cqrs.Commands;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.UserLogout;

public sealed record UserLogoutCommand(string Token) : ICommandVoid;