using Anemoi.BuildingBlock.Application.Cqrs.Commands;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Email, string Token) : ICommandVoid;