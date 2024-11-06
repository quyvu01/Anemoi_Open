using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Workspace.Commands.MemberInvitationCommands.RemoveMemberInvitation;
using Anemoi.Contract.Workspace.Errors;
using AutoMapper;
using MassTransit;
using Serilog;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Cqrs.Commands.MemberInvitationCommands.RemoveMemberInvitation;

public sealed class RemoveMemberInvitationHandler(
    ISqlRepository<MemberInvitation> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger,
    IPublishEndpoint publishEndpoint)
    :
        EfCommandOneVoidHandler<MemberInvitation, RemoveMemberInvitationCommand>(sqlRepository, unitOfWork,
            mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<MemberInvitation> BuildCommand(
        IStartOneCommandVoid<MemberInvitation> fromFlow, RemoveMemberInvitationCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .RemoveOne(x => x.Id == command.Id)
            .WithSpecialAction(null)
            .WithCondition(_ => None.Value)
            .WithErrorIfNull(WorkspaceErrorDetail.MemberInvitationError.NotFound())
            .WithErrorIfSaveChange(WorkspaceErrorDetail.MemberInvitationError.RemoveFailed());
}