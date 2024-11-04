using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Workspace.Commands.WorkspaceCommands.RemoveWorkspace;
using Anemoi.Contract.Workspace.Errors;
using AutoMapper;
using Serilog;

namespace Anemoi.Workspace.Application.Cqrs.Commands.WorkspaceCommands.RemoveWorkspace;

public sealed class RemoveWorkspaceHandler(
    ISqlRepository<Anemoi.Workspace.Domain.Models.Workspace> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandOneVoidHandler<Anemoi.Workspace.Domain.Models.Workspace, RemoveWorkspaceCommand>(sqlRepository, unitOfWork, mapper,
        logger)
{
    protected override ICommandOneFlowBuilderVoid<Anemoi.Workspace.Domain.Models.Workspace> BuildCommand(
        IStartOneCommandVoid<Anemoi.Workspace.Domain.Models.Workspace> fromFlow, RemoveWorkspaceCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .UpdateOne(x => x.Id == command.WorkspaceId)
            .WithSpecialAction(null)
            .WithCondition(_ => None.Value)
            .WithModify(item => item.IsRemoved = true)
            .WithErrorIfNull(WorkspaceErrorDetail.WorkspaceError.NotFound())
            .WithErrorIfSaveChange(WorkspaceErrorDetail.WorkspaceError.UpdateFailed());
}