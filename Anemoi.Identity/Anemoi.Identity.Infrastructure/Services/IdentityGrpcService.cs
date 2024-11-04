using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.Contract.Identity.Commands.IdentityCommands.UserLogout;
using Anemoi.Contract.Identity.Commands.RefreshTokenCommands.UserLogin;
using Anemoi.Grpc.Identity;
using AutoMapper;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Anemoi.Identity.Infrastructure.Services;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class IdentityGrpcService(
    IMapper mapper,
    ILogger logger,
    ISender sender,
    IHttpContextAccessor httpContextAccessor)
    : IdentityService.IdentityServiceBase
{
    [AllowAnonymous]
    public override async Task<AuthenticateResult> Login(LoginRequest request, ServerCallContext context)
    {
        logger.Information("Login with user data: {@Request}", request.UserName);
        var loginResult = await sender.Send(
            new UserLoginCommand(request.UserName, request.Password),
            context.CancellationToken);
        var response = new AuthenticateResult();
        loginResult.Switch(res => response.Succeed = mapper.Map<AuthenticateSucceed>(res), err =>
            response.ErrorDetail = mapper.Map<ErrorDetailResult>(err));
        return response;
    }

    public override async Task<CommandWithVoidResult> LogOut(LogOutRequest request, ServerCallContext context)
    {
        var token = httpContextAccessor.HttpContext.GetToken();
        var result = await sender.Send(new UserLogoutCommand(token));
        var response = new CommandWithVoidResult();
        result.Switch(_ => response.Succeed = new VoidValue(),
            err => response.ErrorDetail = mapper.Map<ErrorDetailResult>(err));
        return response;
    }
}