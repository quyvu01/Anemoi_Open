using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandMany;
using Anemoi.Contract.Workspace.Commands.MemberInvitationCommands.ResendMemberInvitations;
using Anemoi.Contract.Workspace.Errors;
using Anemoi.Orchestration.Contract.EmailSendingContract.Events.EmailSendingRelayEvents;
using AutoMapper;
using MassTransit;
using Serilog;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Cqrs.Commands.MemberInvitationCommands.ResendMemberInvitations;

public sealed class ResendMemberInvitationsHandler(
    ISqlRepository<MemberInvitation> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger,
    IPublishEndpoint publishEndpoint)
    : EfCommandManyVoidHandler<MemberInvitation, ResendMemberInvitationsCommand>(sqlRepository, unitOfWork, mapper,
        logger)
{
    protected override async Task AfterSaveChangesAsync(ResendMemberInvitationsCommand command,
        List<MemberInvitation> models, CancellationToken cancellationToken)
    {
        Logger.Information("[ReCreateMemberInvitation] for: {@MemberInvitations}", models);
        var allTasks = models.Select(x => publishEndpoint
            .Publish<ResendEmailSendingRelay>(new { CorrelationId = x.Id }, cancellationToken));
        await Task.WhenAll(allTasks);
    }

    protected override ICommandManyFlowBuilderVoid<MemberInvitation> BuildCommand(
        IStartManyCommandVoid<MemberInvitation> fromFlow, ResendMemberInvitationsCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .UpdateMany(x => command.Ids.Contains(x.Id))
            .WithSpecialAction(null)
            .WithCondition(_ => None.Value)
            .WithModify(_ => { })
            .WithErrorIfSaveChange(WorkspaceErrorDetail.MemberInvitationError.UpdateFailed());
}