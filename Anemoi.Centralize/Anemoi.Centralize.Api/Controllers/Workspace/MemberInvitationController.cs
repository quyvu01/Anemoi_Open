using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.Workspace.Commands.MemberInvitationCommands.CreateMemberInvitation;
using Anemoi.Contract.Workspace.Commands.MemberInvitationCommands.RemoveMemberInvitation;
using Anemoi.Contract.Workspace.Commands.MemberInvitationCommands.ResendMemberInvitations;
using Anemoi.Contract.Workspace.Queries.MemberInvitationQueries.GetMemberInvitations;
using Anemoi.Contract.Workspace.Responses;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anemoi.Centralize.Api.Controllers.Workspace;

[Route("api/workspace/[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public sealed class MemberInvitationController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Create Member Invitations
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "Agency", Roles = "Administrator")]
    [ProducesResponseType(typeof(PaginationResponse<MemberInvitationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMemberInvitations([FromQuery] GetMemberInvitationsQuery command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return Ok(res);
    }

    /// <summary>
    /// CreateMemberInvitations
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
    public async Task<IActionResult> CreateMemberInvitations([FromBody] CreateMemberInvitationsCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// ResendMemberInvitations
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch]
    [Authorize(Policy = "Agency", Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ResendMemberInvitations([FromBody] ResendMemberInvitationsCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// RemoveMemberInvitation
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    [Authorize(Policy = "Agency", Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveMemberInvitation([FromBody] RemoveMemberInvitationCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }
}