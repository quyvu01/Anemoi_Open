using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Workspace.Errors;
using AutoMapper;
using Serilog;

namespace Anemoi.Workspace.Application.Cqrs.Commands.WorkspaceCommands.UpdateWorkspaceState;

public sealed class UpdateWorkspaceStateHandler(
    ISqlRepository<Domain.Models.Workspace> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandOneVoidHandler<Domain.Models.Workspace, UpdateWorkspaceStateCommand>(sqlRepository, unitOfWork, mapper,
        logger)
{
    protected override ICommandOneFlowBuilderVoid<Domain.Models.Workspace> BuildCommand(
        IStartOneCommandVoid<Domain.Models.Workspace> fromFlow, UpdateWorkspaceStateCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .UpdateOne(x => x.Id == command.WorkspaceId)
            .WithSpecialAction(null)
            .WithCondition(_ => None.Value)
            .WithModify(x => x.State = command.WorkspaceState)
            .WithErrorIfNull(WorkspaceErrorDetail.WorkspaceError.NotFound())
            .WithErrorIfSaveChange(WorkspaceErrorDetail.WorkspaceError.UpdateFailed());
}