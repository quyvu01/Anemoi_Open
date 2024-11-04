using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.ChangePasswordWithToken;

public sealed record ChangePasswordWithTokenCommand(string Email, string Token, string NewPassword) :
    ICommandResult<UserIdResponse>;