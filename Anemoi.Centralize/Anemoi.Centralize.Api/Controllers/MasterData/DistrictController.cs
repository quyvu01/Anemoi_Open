using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.MasterData.Commands.DistrictCommands.CreateDistrict;
using Anemoi.Contract.MasterData.Commands.DistrictCommands.UpdateDistrict;
using Anemoi.Contract.MasterData.ModelIds;
using Anemoi.Contract.MasterData.Queries.DistrictQueries.GetDistrict;
using Anemoi.Contract.MasterData.Queries.DistrictQueries.GetDistricts;
using Anemoi.Contract.MasterData.Responses;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anemoi.Centralize.Api.Controllers.MasterData;

[Route("api/masterData/[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public sealed class DistrictController(ISender sender) : ControllerBase
{
    /// <summary>
    /// This function returns a district by id
    /// </summary>
    /// <param name="query">This is the query object that will be sent to the mediator.</param>
    /// <param name="cancellationToken">This is a token that can be used to cancel the request.</param>
    /// <returns>
    /// A DistrictResponse or an ErrorDetailResponse
    /// </returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(DistrictResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDistrict([FromQuery] GetDistrictQuery query,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(query, cancellationToken);
        return res.Match<IActionResult>(Ok, BadRequest);
    }

    /// <summary>
    /// It returns a list of districts, paginated, filtered by the query parameters
    /// </summary>
    /// <param name="query">This is the query object that will be sent to the mediator.</param>
    /// <param name="cancellationToken">This is a token that can be used to cancel the request.</param>
    /// <returns>
    /// A list of districts
    /// </returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginationResponse<DistrictResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDistricts([FromQuery] GetDistrictsQuery query,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(query, cancellationToken);
        return Ok(res);
    }

    /// <summary>
    /// This function creates a new district
    /// </summary>
    /// <param name="command">This is the command object that will be sent to the
    /// mediator.</param>
    /// <param name="cancellationToken">This is a token that can be used to cancel the request.</param>
    /// <returns>
    /// The result of the command is being returned.
    /// </returns>
    [HttpPost]
    [Authorize(Policy = "Internal", Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateDistrict([FromBody] CreateDistrictCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// Update a district by id
    /// </summary>
    /// <param name="id">The Id of the district to update.</param>
    /// <param name="command">The command object that will be sent to the mediator.</param>
    /// <param name="cancellationToken">This is a token that can be used to cancel the request.</param>
    /// <returns>
    /// The result of the command is being returned.
    /// </returns>
    [HttpPatch("{id}")]
    [Authorize(Policy = "Internal", Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateDistrict(DistrictId id, [FromBody] UpdateDistrictCommand command,
        CancellationToken cancellationToken)
    {
        var request = command with { Id = id };
        var res = await sender.Send(request, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }
}