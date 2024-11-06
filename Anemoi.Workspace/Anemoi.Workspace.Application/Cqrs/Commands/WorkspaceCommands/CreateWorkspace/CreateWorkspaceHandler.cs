using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Workspace.Commands.WorkspaceCommands.CreateWorkspace;
using Anemoi.Contract.Workspace.Errors;
using Anemoi.Contract.Workspace.Responses;
using AutoMapper;
using MassTransit;
using Serilog;

namespace Anemoi.Workspace.Application.Cqrs.Commands.WorkspaceCommands.CreateWorkspace;

public sealed class CreateWorkspaceHandler(
    ISqlRepository<Anemoi.Workspace.Domain.Models.Workspace> sqlRepository,
    IPublishEndpoint publishEndpoint,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandOneResultHandler<Domain.Models.Workspace, CreateWorkspaceCommand, WorkspaceIdResponse>
        (sqlRepository, unitOfWork, mapper, logger)
{
    private const int WorkspaceCountingLimit = 7;
    
    protected override ICommandOneFlowBuilderResult<Domain.Models.Workspace, WorkspaceIdResponse>
        BuildCommand(IStartOneCommandResult<Domain.Models.Workspace, WorkspaceIdResponse> fromFlow,
            CreateWorkspaceCommand command, CancellationToken cancellationToken) => fromFlow
        .CreateOne(Mapper.Map<Domain.Models.Workspace>(command))
        .WithCondition(async workspace =>
        {
            var userWorkspaceCounting = await SqlRepository
                .CountByConditionAsync(a => a.UserId == workspace.UserId, token: cancellationToken);
            if (userWorkspaceCounting >= WorkspaceCountingLimit)
                return WorkspaceErrorDetail.WorkspaceError.WorkspaceExceededLimit();
            return None.Value;
        })
        .WithErrorIfSaveChange(WorkspaceErrorDetail.WorkspaceError.CreateFailed())
        .WithResultIfSucceed(ws => new WorkspaceIdResponse { Id = ws.Id.ToString() });
}