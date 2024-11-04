using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.Identity.Commands.IdentityPolicyCommands.SetUserIdentityPolicy;
using Anemoi.Contract.Identity.Queries.IdentityPolicyQueries.GetIdentityPolicies;
using Anemoi.Contract.Identity.Queries.IdentityPolicyQueries.GetIdentityPolicy;
using Anemoi.Contract.Identity.Responses;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anemoi.Centralize.Api.Controllers.Identity;

[Route("api/identity/[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public class IdentityPolicyController(ISender sender) : ControllerBase
{
    /// <summary>
    /// GetIdentityPolicyRoles
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserRoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetIdentityPolicyRoles(CancellationToken cancellationToken)
    {
        var res = await sender.Send(new GetIdentityPolicyQuery(), cancellationToken);
        return res.Match<IActionResult>(x => Ok(x.UserRoles), BadRequest);
    }

    /// <summary>
    /// GetIdentityPolicies
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "Internal", Roles = "Administrator")]
    [ProducesResponseType(typeof(CollectionResponse<IdentityPolicyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetIdentityPolicies(CancellationToken cancellationToken)
    {
        var res = await sender
            .Send(new GetIdentityPoliciesQuery(), cancellationToken);
        return Ok(res);
    }

    /// <summary>
    /// SetUserIdentityPolicy
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch]
    [Authorize(Policy = "InternalOrAgency", Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SetUserIdentityPolicy([FromBody] SetUserIdentityPolicyCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }
}