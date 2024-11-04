using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Commands.RefreshTokenCommands.UserLogin;

public sealed record UserLoginCommand(string UserName, string Password) :
    ICommandResult<AuthenticationSuccessResponse>;