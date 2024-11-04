using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.Notification.Commands.EmailConfigurationCommands.CreateEmailConfiguration;
using Anemoi.Contract.Notification.Commands.EmailConfigurationCommands.RemoveEmailConfigurations;
using Anemoi.Contract.Notification.Commands.EmailConfigurationCommands.SendTemporaryEmail;
using Anemoi.Contract.Notification.Commands.EmailConfigurationCommands.UpdateEmailConfiguration;
using Anemoi.Contract.Notification.ModelIds;
using Anemoi.Contract.Notification.Queries.EmailConfigurationQueries.GetDefaultEmailConfiguration;
using Anemoi.Contract.Notification.Queries.EmailConfigurationQueries.GetEmailConfiguration;
using Anemoi.Contract.Notification.Queries.EmailConfigurationQueries.GetEmailConfigurationsByWorkspace;
using Anemoi.Contract.Notification.Responses;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anemoi.Centralize.Api.Controllers.Notification;

[Route("api/notification/[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public sealed class EmailConfigurationController(ISender sender) : ControllerBase
{
    /// <summary>
    /// GetEmailConfigurationByWorkspaceRequest
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "Agency", Roles = "Administrator")]
    [ProducesResponseType(typeof(EmailConfigurationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDefaultEmailConfiguration(CancellationToken cancellationToken)
    {
        var res = await sender
            .Send(new GetDefaultEmailConfigurationQuery(HttpContext.GetWorkspaceId()), cancellationToken);
        return res.Match<IActionResult>(Ok, BadRequest);
    }

    /// <summary>
    /// GetEmailConfiguration
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "Agency", Roles = "Administrator")]
    [ProducesResponseType(typeof(EmailConfigurationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEmailConfiguration(GetEmailConfigurationQuery query,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(query, cancellationToken);
        return res.Match<IActionResult>(Ok, BadRequest);
    }

    /// <summary>
    /// GetEmailConfigurationsByWorkspaceQuery
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "Agency", Roles = "Administrator")]
    [ProducesResponseType(typeof(PaginationResponse<EmailConfigurationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEmailConfigurationsByWorkspace(
        GetEmailConfigurationsByWorkspaceQuery query, CancellationToken cancellationToken)
    {
        query.WorkspaceId = HttpContext.GetWorkspaceId();
        var res = await sender.Send(query, cancellationToken);
        return Ok(res);
    }

    /// <summary>
    /// CreateEmailConfiguration
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
    public async Task<IActionResult> CreateEmailConfiguration(
        [FromBody] CreateEmailConfigurationCommand command, CancellationToken cancellationToken)
    {
        var res = await sender
            .Send(command, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// UpdateEmailConfiguration
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
    public async Task<IActionResult> UpdateEmailConfiguration(EmailConfigurationId id,
        [FromBody] UpdateEmailConfigurationCommand command, CancellationToken cancellationToken)
    {
        var res = await sender.Send(command with { Id = id }, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// SendTemporaryEmail
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
    public async Task<IActionResult> SendTemporaryEmail(
        [FromBody] SendTemporaryEmailCommand command, CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    [HttpDelete]
    [Authorize(Policy = "Agency", Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveEmailConfigurations(
        [FromBody] RemoveEmailConfigurationsCommand command, CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }
}