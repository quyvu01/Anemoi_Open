using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Workspace.Commands.WorkspaceCommands.UpdateWorkspace;
using Anemoi.Contract.Workspace.Errors;
using AutoMapper;
using Serilog;

namespace Anemoi.Workspace.Application.Cqrs.Commands.WorkspaceCommands.UpdateWorkspace;

public sealed class UpdateWorkspaceHandler(
    ISqlRepository<Anemoi.Workspace.Domain.Models.Workspace> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandOneVoidHandler<Anemoi.Workspace.Domain.Models.Workspace, UpdateWorkspaceCommand>(sqlRepository,
        unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<Anemoi.Workspace.Domain.Models.Workspace> BuildCommand(
        IStartOneCommandVoid<Anemoi.Workspace.Domain.Models.Workspace> fromFlow, UpdateWorkspaceCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .UpdateOne(x => x.Id == command.Id)
            .WithSpecialAction(null)
            .WithCondition(existOne => None.Value)
            .WithModify(workspace => Mapper.Map(command, workspace))
            .WithErrorIfNull(WorkspaceErrorDetail.WorkspaceError.NotFound())
            .WithErrorIfSaveChange(WorkspaceErrorDetail.WorkspaceError.UpdateFailed());
}