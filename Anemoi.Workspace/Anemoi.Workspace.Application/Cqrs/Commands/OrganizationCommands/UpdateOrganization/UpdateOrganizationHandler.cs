using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Workspace.Commands.OrganizationCommands.UpdateOrganization;
using Anemoi.Contract.Workspace.Errors;
using AutoMapper;
using Serilog;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Cqrs.Commands.OrganizationCommands.UpdateOrganization;

public sealed class UpdateOrganizationHandler(
    ISqlRepository<Organization> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandOneVoidHandler<Organization, UpdateOrganizationCommand>(sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<Organization> BuildCommand(
        IStartOneCommandVoid<Organization> fromFlow, UpdateOrganizationCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .UpdateOne(x => x.Id == command.Id)
            .WithSpecialAction(null)
            .WithCondition(_ => None.Value)
            .WithModify(organization => Mapper.Map(command, organization))
            .WithErrorIfNull(WorkspaceErrorDetail.OrganizationError.NotFound())
            .WithErrorIfSaveChange(WorkspaceErrorDetail.OrganizationError.UpdateFailed());
}