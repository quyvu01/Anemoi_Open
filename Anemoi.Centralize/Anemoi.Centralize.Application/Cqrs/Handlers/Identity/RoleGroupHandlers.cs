using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.Centralize.Application.Abstractions;
using Anemoi.Centralize.Application.Cqrs.Requests.Workspace;
using Anemoi.Contract.Identity.Commands.RoleGroupCommands.CreateRoleGroup;
using Anemoi.Contract.Identity.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http;
using OneOf;

namespace Anemoi.Centralize.Application.Cqrs.Handlers.Identity;

public sealed class RoleGroupHandlers(
    IRequestClientService requestClientService,
    ISender sender,
    IHttpContextAccessor httpContextAccessor)
    : ICommandVoidHandler<CreateWorkspaceRoleGroupRequest>
{
    public IRequestClientService RequestClientService { get; } = requestClientService;

    public Task<OneOf<None, ErrorDetailResponse>> Handle(CreateWorkspaceRoleGroupRequest request,
        CancellationToken cancellationToken)
    {
        var workspaceId = httpContextAccessor.HttpContext.GetWorkspaceId();
        return sender.Send(new CreateRoleGroupCommand(request.Name, request.Description,
                [new RoleGroupClaimContract(nameof(workspaceId), workspaceId)], request.IdentityRoleIds, false),
            cancellationToken);
    }
}