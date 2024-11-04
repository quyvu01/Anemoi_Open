using Anemoi.BuildingBlock.Application.Cqrs.Commands;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.CheckEmailResetTokenCommand;

public sealed record CheckEmailResetTokenCommand(string Email, string Code) : ICommandVoid;