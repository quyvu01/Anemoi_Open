using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.Workspace.Commands.OrganizationCommands.CreateOrganization;
using Anemoi.Contract.Workspace.Commands.OrganizationCommands.UpdateOrganization;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Queries.OrganizationQueries.CheckIfSubDomainIsValid;
using Anemoi.Contract.Workspace.Queries.OrganizationQueries.GetOrganization;
using Anemoi.Contract.Workspace.Queries.OrganizationQueries.GetOrganizations;
using Anemoi.Contract.Workspace.Responses;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anemoi.Centralize.Api.Controllers.Workspace;

[Route("api/workspace/[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public sealed class OrganizationController(ISender sender) : ControllerBase
{
    /// <summary>
    /// GetOrganization
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "Agency")]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOrganization([FromQuery] GetOrganizationQuery query,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(query, cancellationToken);
        return res.Match<IActionResult>(Ok, BadRequest);
    }

    /// <summary>
    /// CheckIfSubDomainIsValid
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "Agency")]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckIfSubDomainIsValid([FromQuery] CheckIfSubDomainIsValidQuery query,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(query, cancellationToken);
        var isValid = res.IsT1;
        return Ok(isValid);
    }

    /// <summary>
    /// GetOrganizationByWorkspace
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "Agency")]
    [ProducesResponseType(typeof(PaginationResponse<OrganizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOrganizations([FromQuery] GetOrganizationsQuery query,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(query, cancellationToken);
        return Ok(res);
    }

    /// <summary>
    /// CreateOrganization
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "Agency", Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// UpdateOrganization
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    [Authorize(Policy = "Agency", Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateOrganization(OrganizationId id, [FromBody] UpdateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command with { Id = id }, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }
}