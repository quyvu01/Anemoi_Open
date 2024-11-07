using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.MasterData.Commands.SampleAddressCommands.CreateSampleAddress;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anemoi.Centralize.Api.Controllers.MasterData;

[Route("api/masterData/[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public sealed class AdminController(ISender sender) : ControllerBase
{
    /// <summary>
    /// > This function creates a sample address and returns the result as a 200 OK response
    /// </summary>
    /// <param name="cancellationToken">This is a standard .NET cancellation token that can be used to
    /// cancel the request.</param>
    /// <returns>
    /// Either an Ok or BadRequest
    /// </returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateSampleAddress(CancellationToken cancellationToken)
    {
        var res = await sender.Send(new CreateSampleAddressCommand(), cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }
}