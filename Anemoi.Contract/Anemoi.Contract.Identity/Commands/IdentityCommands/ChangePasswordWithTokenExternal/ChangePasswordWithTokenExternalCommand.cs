using Anemoi.BuildingBlock.Application.Cqrs.Commands;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.ChangePasswordWithTokenExternal;

public sealed record ChangePasswordWithTokenExternalCommand
    (string Email, string Token, string NewPassword) : ICommandVoid;