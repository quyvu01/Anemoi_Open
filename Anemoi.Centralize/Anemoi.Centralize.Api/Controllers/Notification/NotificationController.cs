using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.Notification.Commands.NotificationCommands.MarkAsReadNotification;
using Anemoi.Contract.Notification.Commands.NotificationCommands.RemoveNotification;
using Anemoi.Contract.Notification.Queries.NotificationQueries.GetCountingUnreadNotifications;
using Anemoi.Contract.Notification.Queries.NotificationQueries.GetNotifications;
using Anemoi.Contract.Notification.Responses;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anemoi.Centralize.Api.Controllers.Notification;

[Route("api/notification/[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public sealed class NotificationController(ISender sender) : ControllerBase
{
    /// <summary>
    /// GetNotifications
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(PaginationResponse<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetNotifications([FromQuery] GetNotificationsQuery query,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(query, cancellationToken);
        return Ok(res);
    }

    /// <summary>
    /// GetCountingUnreadNotifications
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(CountingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUnreadCountingNotifications(CancellationToken cancellationToken)
    {
        var res = await sender.Send(new GetCountingUnreadNotificationsQuery(), cancellationToken);
        return Ok(res);
    }

    /// <summary>
    /// MarkAsReadNotification
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MarkAsReadNotification([FromBody] MarkAsReadNotificationCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// RemoveNotification
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveNotification([FromBody] RemoveNotificationCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }
}