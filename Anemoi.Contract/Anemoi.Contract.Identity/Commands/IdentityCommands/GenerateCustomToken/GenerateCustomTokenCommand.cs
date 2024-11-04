using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.GenerateCustomToken;

public sealed record GenerateCustomTokenCommand(List<ClaimRequestData> ClaimsRequest, TimeSpan? ExpiredIn) :
    ICommandResult<CustomTokenResponse>;