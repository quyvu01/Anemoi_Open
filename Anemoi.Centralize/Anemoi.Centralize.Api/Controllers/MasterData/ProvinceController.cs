using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.MasterData.Commands.ProvinceCommands.CreateProvince;
using Anemoi.Contract.MasterData.Commands.ProvinceCommands.UpdateProvince;
using Anemoi.Contract.MasterData.ModelIds;
using Anemoi.Contract.MasterData.Queries.ProvinceQueries.GetProvince;
using Anemoi.Contract.MasterData.Queries.ProvinceQueries.GetProvinces;
using Anemoi.Contract.MasterData.Responses;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anemoi.Centralize.Api.Controllers.MasterData;

[Route("api/masterData/[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public sealed class ProvinceController(ISender sender) : ControllerBase
{
    /// <summary>
    /// This function returns a list of provinces from the database
    /// </summary>
    /// <param name="query">This is the query object that will be passed to the handler.</param>
    /// <param name="cancellationToken">This is a token that can be used to cancel the request.</param>
    /// <returns>
    /// A ProvinceResponse or an ErrorDetailResponse
    /// </returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProvinceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetProvince([FromQuery] GetProvinceQuery query,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(query, cancellationToken);
        return res.Match<IActionResult>(Ok, BadRequest);
    }

    /// <summary>
    /// It returns a list of provinces, paginated, and it's accessible to anonymous users
    /// </summary>
    /// <param name="query">This is the query object that will be passed to the handler.</param>
    /// <param name="cancellationToken">This is a token that can be used to cancel the request.</param>
    /// <returns>
    /// A list of provinces
    /// </returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginationResponse<ProvinceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetProvinces([FromQuery] GetProvincesQuery query,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(query, cancellationToken);
        return Ok(res);
    }

    /// <summary>
    /// This function creates a new province and returns the result
    /// </summary>
    /// <param name="command">The command object that will be sent to the mediator.</param>
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
    public async Task<IActionResult> CreateProvince([FromBody] CreateProvinceCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// Update a province by id
    /// </summary>
    /// <param name="id">The Id of the province to update.</param>
    /// <param name="command">The command object that will be sent to the mediator.</param>
    /// <param name="cancellationToken">This is a token that can be used to cancel the request.</param>
    /// <returns>
    /// The result of the command is being returned.
    /// </returns>
    [HttpPatch("{id}")]
    [Authorize(Policy = "Internal", Roles = "Administrator")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateProvince(ProvinceId id, [FromBody] UpdateProvinceCommand command,
        CancellationToken cancellationToken)
    {
        var request = command with { Id = id };
        var res = await sender.Send(request, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }
}