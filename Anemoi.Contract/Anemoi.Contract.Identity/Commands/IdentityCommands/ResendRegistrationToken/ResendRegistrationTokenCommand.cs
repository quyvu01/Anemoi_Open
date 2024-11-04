using Anemoi.BuildingBlock.Application.Cqrs.Commands;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.ResendRegistrationToken;

public sealed record ResendRegistrationTokenCommand(string Email) : ICommandVoid;