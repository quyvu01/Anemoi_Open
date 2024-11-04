using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandMany;
using Anemoi.Contract.Identity.Queries.UserQueries.GetUserWithEmailsByEmails;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Contract.Workspace.Commands.MemberInvitationCommands.CreateMemberInvitation;
using Anemoi.Contract.Workspace.Errors;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Orchestration.Contract.EmailSendingContract.Events.EmailSendingRelayEvents;
using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Serilog;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Cqrs.Commands.MemberInvitationCommands.CreateMemberInvitations;

public sealed class CreateMemberInvitationsHandler(
    ISqlRepository<MemberInvitation> sqlRepository,
    ISqlRepository<Member> memberRepository,
    IRequestClient<GetUserWithEmailsByEmailsQuery> userWithEmailsByEmailsClient,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger,
    IPublishEndpoint publishEndpoint,
    IWorkspaceIdGetter workspaceIdGetter)
    : EfCommandManyVoidHandler<MemberInvitation, CreateMemberInvitationsCommand>(sqlRepository, unitOfWork, mapper,
        logger)
{
    protected override async Task AfterSaveChangesAsync(CreateMemberInvitationsCommand command,
        List<MemberInvitation> models, CancellationToken cancellationToken)
    {
        var allTasks = models
            .Select(x => publishEndpoint.Publish<CreateEmailSendingRelay>(new
            {
                CorrelationId = x.Id.Value, Router = "memberInvitation",
                EmailTo = x.Email, Parameters = new Dictionary<string, string>
                {
                    { "WorkspaceId", x.WorkspaceId.ToString() },
                    { "MemberInvitationId", x.Id.ToString() },
                    { "CreatorId", x.CreatorId }
                }
            }, cancellationToken));
        await Task.WhenAll(allTasks);
    }

    public override async Task<OneOf<None, ErrorDetailResponse>> Handle(CreateMemberInvitationsCommand request,
        CancellationToken cancellationToken)
    {
        var userEmails = request.Users.Select(a => a.Email).Distinct().ToList();
        if (userEmails.Count != request.Users.Count)
            return WorkspaceErrorDetail.MemberInvitationError.AlreadyExist().ToErrorDetailResponse();
        var workspaceId = new WorkspaceId(Guid.Parse(workspaceIdGetter.WorkspaceId));

        var usersData = await SqlRepository
            .GetQueryable(x =>
                x.WorkspaceId == workspaceId && request.Users.Any(a => a.Email == x.Email))
            .Select(x => x.Email)
            .ToListAsync(cancellationToken: cancellationToken);

        var usersResponse = await userWithEmailsByEmailsClient
            .GetResponse<CollectionResponse<UserResponse>>(new GetUserWithEmailsByEmailsQuery(userEmails.ToList()),
                cancellationToken);

        var userIds = usersResponse.Message.Items.Select(x => x.UserId);
        var memberUserIds = await memberRepository
            .GetQueryable(a => userIds.Contains(a.UserId))
            .AsNoTracking()
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);
        var usersExistOnMembers = usersResponse.Message.Items
            .Where(x => memberUserIds.Contains(x.UserId))
            .Select(x => x.Email);

        var allExistedEmails = usersData.Concat(usersExistOnMembers);

        var invitations = request.Users
            .Where(x => allExistedEmails.All(email => email != x.Email))
            .ToList();
        var newCommand = new CreateMemberInvitationsCommand(invitations);
        return await base.Handle(newCommand, cancellationToken);
    }


    protected override ICommandManyFlowBuilderVoid<MemberInvitation> BuildCommand(
        IStartManyCommandVoid<MemberInvitation> fromFlow, CreateMemberInvitationsCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .CreateMany(Mapper.Map<List<MemberInvitation>>(command.Users))
            .WithCondition(_ => None.Value)
            .WithErrorIfSaveChange(WorkspaceErrorDetail.MemberInvitationError.CreateFailed());
}