using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.Contracts;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Contract.Identity.Commands.RefreshTokenCommands.GenerateTokenByRoleGroupClaims;

public sealed record GenerateTokenByRoleGroupClaimsCommand(List<RoleGroupClaimContract> RoleGroupClaims)
    : ICommandResult<AuthenticationSuccessResponse>;