using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.Workspace.Commands.WorkspaceCommands.CreateWorkspace;
using Anemoi.Contract.Workspace.Commands.WorkspaceCommands.UpdateWorkspace;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetNumOfWorkspaces;
using Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetWorkspace;
using Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetWorkspaces;
using Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetWorkspacesByUser;
using Anemoi.Contract.Workspace.Responses;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anemoi.Centralize.Api.Controllers.Workspace;

[Route("api/workspace/[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public sealed class WorkspaceController(ISender sender) : ControllerBase
{
    /// <summary>
    /// GetWorkspace
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "Agency")]
    [ProducesResponseType(typeof(WorkspaceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetWorkspace([FromQuery] GetWorkspaceQuery query,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(query, cancellationToken);
        return res.Match<IActionResult>(Ok, BadRequest);
    }

    /// <summary>
    /// GetNumOfWorkspaces
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "Agency")]
    [ProducesResponseType(typeof(CountingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetNumOfWorkspaces(CancellationToken cancellationToken)
    {
        var res = await sender.Send(new GetNumOfWorkspacesQuery(), cancellationToken);
        return Ok(res);
    }

    /// <summary>
    /// GetWorkspaceByUser
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "Agency")]
    [ProducesResponseType(typeof(PaginationResponse<WorkspaceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetWorkspacesByUser([FromQuery] GetWorkspacesByUserQuery query,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(query, cancellationToken);
        return Ok(res);
    }

    /// <summary>
    /// GetWorkspaces
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "Internal")]
    [Authorize]
    [ProducesResponseType(typeof(PaginationResponse<WorkspaceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetWorkspaces([FromQuery] GetWorkspacesQuery query,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(query, cancellationToken);
        return Ok(res);
    }

    /// <summary>
    /// Create Workspace
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "Agency")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(Ok, BadRequest);
    }

    /// <summary>
    /// Update Workspace
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateWorkspace(WorkspaceId id, [FromBody] UpdateWorkspaceCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender
            .Send(command with { Id = id }, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }
}