using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.UserChangePassword;

public sealed record UserChangePasswordCommand(string CurrentPassword, string NewPassword) :
    ICommandResult<UserIdResponse>;