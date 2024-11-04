using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Queries.UserQueries.GetUserWithEmailsByEmails;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Contract.Workspace.Commands.MemberCommands.CreateMember;
using Anemoi.Contract.Workspace.Errors;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Responses;
using Anemoi.Orchestration.Contract.WorkspaceContract.Events.MemberRoleGroupSynchronizedEvents;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Serilog;
using Anemoi.Workspace.Domain.Models;
using MassTransit;

namespace Anemoi.Workspace.Application.Cqrs.Commands.MemberCommands.CreateMember;

public sealed class CreateMemberHandler(
    ISqlRepository<Member> sqlRepository,
    ISqlRepository<MemberInvitation> memberInvitationRepository,
    IRequestClient<GetUserWithEmailsByEmailsQuery> getUserByEmailClient,
    IPublishEndpoint publishEndpoint,
    IWorkspaceIdGetter workspaceIdGetter,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandOneResultHandler<Member, CreateMemberCommand, MemberIdResponse>(sqlRepository, unitOfWork, mapper,
        logger)
{
    protected override async Task AfterSaveChangesAsync(CreateMemberCommand command, Member model,
        MemberIdResponse result, CancellationToken cancellationToken)
    {
        var memberInvitation = await memberInvitationRepository
            .GetQueryable(x => x.Id == command.MemberInvitationId)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        await publishEndpoint.Publish<CreateMemberRoleGroupSynchronized>(new
        {
            workspaceIdGetter.WorkspaceId, model.UserId, MemberId = model.Id.Value, memberInvitation.RoleGroupIds
        }, cancellationToken);
    }

    public override async Task<OneOf<MemberIdResponse, ErrorDetailResponse>> Handle(CreateMemberCommand request,
        CancellationToken cancellationToken)
    {
        var memberInvitation = await memberInvitationRepository
            .GetQueryable(x => x.Id == request.MemberInvitationId)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        if (memberInvitation is null)
            return WorkspaceErrorDetail.MemberInvitationError.NotFound().ToErrorDetailResponse();
        var userEmail = memberInvitation.Email;
        var usersResponse = await getUserByEmailClient
            .GetResponse<CollectionResponse<UserResponse>>(new GetUserWithEmailsByEmailsQuery([userEmail]),
                cancellationToken);
        var userId = usersResponse.Message.Items.FirstOrDefault()?.UserId;
        if (userId is null) return WorkspaceErrorDetail.MemberError.UserNotFound().ToErrorDetailResponse();
        var workspaceId = new WorkspaceId(Guid.Parse(workspaceIdGetter.WorkspaceId));
        var member = await SqlRepository
            .GetFirstByConditionAsync(x => x.WorkspaceId == workspaceId && x.UserId == userId,
                db => db.AsNoTracking(), token: cancellationToken);
        if (member is not null) return Mapper.Map<MemberIdResponse>(member);
        return await base.Handle(request with { UserId = userId }, cancellationToken);
    }

    protected override ICommandOneFlowBuilderResult<Member, MemberIdResponse> BuildCommand(
        IStartOneCommandResult<Member, MemberIdResponse> fromFlow, CreateMemberCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .CreateOne(Mapper.Map<Member>(command))
            .WithCondition(_ => None.Value)
            .WithErrorIfSaveChange(WorkspaceErrorDetail.MemberError.CreateFailed())
            .WithResultIfSucceed(data => Mapper.Map<MemberIdResponse>(data));
}