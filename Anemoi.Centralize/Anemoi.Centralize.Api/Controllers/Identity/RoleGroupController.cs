using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Centralize.Application.Cqrs.Requests.Workspace;
using Anemoi.Contract.Identity.Commands.RoleGroupCommands.RemoveRoleGroup;
using Anemoi.Contract.Identity.Commands.RoleGroupCommands.UpdateRoleGroup;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Contract.Identity.Queries.RoleGroupQueries.GetRoleGroup;
using Anemoi.Contract.Identity.Queries.RoleGroupQueries.GetRoleGroups;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Queries.MemberMapRoleGroupQueries.GetRoleGroupsByMember;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anemoi.Centralize.Api.Controllers.Identity;

[Route("api/identity/[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public sealed class RoleGroupController(ISender sender) : ControllerBase
{
    /// <summary>
    /// This function returns a list of role groups
    /// </summary>
    /// <param name="query">This is the query object that will be sent to the mediator.</param>
    /// <param name="cancellationToken">This is a token that can be used to cancel the request.</param>
    /// <returns>
    /// A list of RoleGroupListResponse objects.
    /// </returns>
    [HttpGet]
    [Authorize(Roles = "Administrator,RoleQuery")]
    [ProducesResponseType(typeof(RoleGroupsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRoleGroup([FromQuery] GetRoleGroupQuery query,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(query, cancellationToken);
        return result.Match<IActionResult>(Ok, BadRequest);
    }

    /// <summary>
    /// GetRoleGroupsByMember
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "Agency")]
    [ProducesResponseType(typeof(RoleGroupsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRoleGroupsByMember(CancellationToken cancellationToken)
    {
        var workspaceId = new WorkspaceId(Guid.Parse(HttpContext.GetWorkspaceId()));
        var result = await sender
            .Send(new GetRoleGroupsByMemberQuery(HttpContext.GetUserId(), workspaceId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// GetRoleGroupsByWorkspaceMember
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "Agency")]
    [ProducesResponseType(typeof(RoleGroupsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRoleGroupsByWorkspaceMember([FromQuery] GetRoleGroupsByMemberQuery query,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// This function returns a list of role groups
    /// </summary>
    /// <param name="query">This is the query object that will be passed to the
    /// mediator.</param>
    /// <param name="cancellationToken">This is a token that can be used to cancel the request.</param>
    /// <returns>
    /// A list of role groups.
    /// </returns>
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(typeof(PaginationResponse<RoleGroupsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRoleGroups([FromQuery] GetRoleGroupsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// This function creates a role group
    /// </summary>
    /// <param name="command">This is the command object that will be sent to the
    /// mediator.</param>
    /// <param name="cancellationToken">This is a token that can be used to cancel the request.</param>
    /// <returns>
    /// The result of the command is being returned.
    /// </returns>
    [HttpPost]
    [Authorize(Roles = "Administrator,RoleCommand")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateWorkspaceRoleGroup([FromBody] CreateWorkspaceRoleGroupRequest command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// Update a role group
    /// </summary>
    /// <param name="id">This is a custom type that is used to represent the id of a role group.
    /// It's a simple class that inherits from the `Id` class.</param>
    /// <param name="command">The command object that will be sent to the mediator.</param>
    /// <param name="cancellationToken">This is a token that can be used to cancel the request.</param>
    /// <returns>
    /// The result of the command is being returned.
    /// </returns>
    [HttpPatch("{id}")]
    [Authorize(Roles = "Administrator,RoleCommand")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateRoleGroup(RoleGroupId id, [FromBody] UpdateRoleGroupCommand command,
        CancellationToken cancellationToken)
    {
        var request = command with { Id = id };
        var res = await sender.Send(request, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// Remove the specified role group from the specified role
    /// </summary>
    /// <param name="command">This is the command object that will be sent to the
    /// mediator.</param>
    /// <param name="cancellationToken">This is a token that can be used to cancel the request.</param>
    /// <returns>
    /// The result of the command.
    /// </returns>
    [HttpDelete]
    [Authorize(Roles = "Administrator,RoleCommand")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveRoleGroup([FromBody] RemoveRoleGroupCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }
}