using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Centralize.Application.Cqrs.Requests.Workspace;
using Anemoi.Contract.Identity.Commands.IdentityCommands.CheckEmailResetTokenCommand;
using Anemoi.Contract.Identity.Commands.IdentityCommands.ConfirmEmail;
using Anemoi.Contract.Identity.Commands.IdentityCommands.ResendRegistrationToken;
using Anemoi.Contract.Identity.Commands.IdentityCommands.UserChangePassword;
using Anemoi.Contract.Identity.Commands.IdentityCommands.UserForgetPassword;
using Anemoi.Contract.Identity.Commands.IdentityCommands.UserLogout;
using Anemoi.Contract.Identity.Commands.RefreshTokenCommands.GenerateTokenByRoleGroupClaims;
using Anemoi.Contract.Identity.Commands.RefreshTokenCommands.UserLogin;
using Anemoi.Contract.Identity.Commands.RefreshTokenCommands.UserRefreshToken;
using Anemoi.Contract.Identity.Commands.UserCommands.CreateUser;
using Anemoi.Contract.Identity.Contracts;
using Anemoi.Contract.Identity.Queries.IdentityQueries.CheckUserExist;
using Anemoi.Contract.Identity.Responses;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anemoi.Centralize.Api.Controllers.Identity;

[Route("api/identity/[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public class IdentityController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Check Account
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserWithEmailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckAccount([FromQuery] CheckUserExistQuery query,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(query, cancellationToken);
        return result.Match<IActionResult>(Ok, BadRequest);
    }

    /// <summary>
    /// Active Account
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActiveAccount([FromQuery] ConfirmEmailCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationSuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] UserLoginCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(Ok, BadRequest);
    }

    /// <summary>
    /// GenerateWorkspaceToken
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(AuthenticationSuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateWorkspaceToken([FromBody] GenerateWorkspaceTokenRequest command,
        CancellationToken cancellationToken)
    {
        var res = await sender
            .Send(new GenerateTokenByRoleGroupClaimsCommand([
                new RoleGroupClaimContract("workspaceId", command.WorkspaceId)
            ]), cancellationToken);
        return res.Match<IActionResult>(Ok, BadRequest);
    }

    /// <summary>
    /// RefreshToken
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationSuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] UserRefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(command, cancellationToken);
        return res.Match<IActionResult>(Ok, BadRequest);
    }


    /// <summary>
    /// Logout
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(AuthenticationSuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var res = await sender
            .Send(new UserLogoutCommand(HttpContext.GetToken()), cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// Register Account
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RegisterAccount([FromBody] CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(request, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// ResendRegistrationToken
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ResendRegistrationToken([FromBody] ResendRegistrationTokenCommand request,
        CancellationToken cancellationToken)
    {
        var res = await sender.Send(request, cancellationToken);
        return res.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// Change Password
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] UserChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// Forget Password
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ForgetPassword([FromBody] UserForgetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.Match<IActionResult>(_ => Ok(), BadRequest);
    }

    /// <summary>
    /// Check Email Token
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckEmailResetToken(
        [FromBody] CheckEmailResetTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.Match<IActionResult>(_ => Ok(), BadRequest);
    }
}