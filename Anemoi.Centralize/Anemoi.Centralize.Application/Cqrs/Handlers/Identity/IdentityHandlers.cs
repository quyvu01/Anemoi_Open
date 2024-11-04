using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.Centralize.Application.Abstractions;
using Anemoi.Centralize.Application.Configurations;
using Anemoi.Contract.Identity.Commands.IdentityCommands.UserLogout;
using Anemoi.Contract.Identity.Commands.RefreshTokenCommands.UserLogin;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Grpc.Identity;
using Anemoi.Grpc.Identity.Client;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using OneOf;

namespace Anemoi.Centralize.Application.Cqrs.Handlers.Identity;

public sealed class IdentityHandlers(
    IRequestClientService requestClientService,
    IMapper mapper,
    GrpcSetting grpcSetting,
    IHttpContextAccessor httpContextAccessor)
    :
        ICommandResultHandler<UserLoginCommand, AuthenticationSuccessResponse>,
        ICommandVoidHandler<UserLogoutCommand>
{
    public IRequestClientService RequestClientService { get; } = requestClientService;

    public async Task<OneOf<None, ErrorDetailResponse>> Handle(UserLogoutCommand request,
        CancellationToken cancellationToken)
    {
        var result = await IdentityClientServices.LogOutAsync(grpcSetting.IdentityAddress,
            httpContextAccessor.HttpContext.GetToken(), cancellationToken);
        if (result.ResultCase == CommandWithVoidResult.ResultOneofCase.Succeed) return None.Value;
        return mapper.Map<ErrorDetailResponse>(result.ErrorDetail);
    }

    public async Task<OneOf<AuthenticationSuccessResponse, ErrorDetailResponse>> Handle(UserLoginCommand request,
        CancellationToken cancellationToken)
    {
        var loginRequest = new LoginRequest { UserName = request.UserName, Password = request.Password };
        var result = await IdentityClientServices
            .LoginAsync(grpcSetting.IdentityAddress, loginRequest, cancellationToken);
        if (result.ResultCase != AuthenticateResult.ResultOneofCase.Succeed)
            return mapper.Map<ErrorDetailResponse>(result.ErrorDetail);
        return mapper.Map<AuthenticationSuccessResponse>(result.Succeed);
    }
}