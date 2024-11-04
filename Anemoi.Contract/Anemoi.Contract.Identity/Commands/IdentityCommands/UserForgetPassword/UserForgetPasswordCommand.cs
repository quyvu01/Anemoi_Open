using Anemoi.BuildingBlock.Application.Cqrs.Commands;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.UserForgetPassword;

public record UserForgetPasswordCommand(string Email) : ICommandVoid;