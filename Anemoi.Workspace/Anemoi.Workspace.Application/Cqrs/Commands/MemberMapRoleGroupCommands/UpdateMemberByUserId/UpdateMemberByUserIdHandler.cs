using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandMany;
using Anemoi.Contract.Workspace.Commands.MemberMapRoleGroupCommands.UpdateMemberByUserId;
using Anemoi.Contract.Workspace.Errors;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Workspace.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Anemoi.Workspace.Application.Cqrs.Commands.MemberMapRoleGroupCommands.UpdateMemberByUserId;

public sealed class UpdateMemberByUserIdHandler(
    ISqlRepository<MemberMapRoleGroup> sqlRepository,
    ISqlRepository<Member> memberRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandManyVoidHandler<MemberMapRoleGroup, UpdateMemberByUserIdCommand>(sqlRepository, unitOfWork, mapper,
        logger)
{
    protected override ICommandManyFlowBuilderVoid<MemberMapRoleGroup> BuildCommand(
        IStartManyCommandVoid<MemberMapRoleGroup> fromFlow, UpdateMemberByUserIdCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .CreateMany(async () =>
            {
                var exists = await SqlRepository
                    .GetManyByConditionAsync(a =>
                            a.Member.UserId == command.UserId && a.Member.WorkspaceId == command.WorkspaceId,
                        token: cancellationToken);
                await SqlRepository.RemoveManyAsync(exists, cancellationToken);
                var memberId = await memberRepository
                    .GetQueryable(a => a.UserId == command.UserId && a.WorkspaceId == command.WorkspaceId)
                    .AsNoTracking()
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                return command.RoleGroupIds.Select((x, index) => new MemberMapRoleGroup
                {
                    Id = new MemberMapRoleGroupId(IdGenerator.NextGuid()), RoleGroupId = x, MemberId = memberId,
                    Order = index
                }).ToList();
            })
            .WithCondition(_ => None.Value)
            .WithErrorIfSaveChange(WorkspaceErrorDetail.MemberMapRoleGroupError.CreateFailed());
}