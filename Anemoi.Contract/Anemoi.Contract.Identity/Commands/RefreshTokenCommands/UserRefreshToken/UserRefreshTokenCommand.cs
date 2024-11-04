using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Commands.RefreshTokenCommands.UserRefreshToken;

public sealed record UserRefreshTokenCommand(RefreshTokenId RefreshToken)
    : ICommandResult<AuthenticationSuccessResponse>;