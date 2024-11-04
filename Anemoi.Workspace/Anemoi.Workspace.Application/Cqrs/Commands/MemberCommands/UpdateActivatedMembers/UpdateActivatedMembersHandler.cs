using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandMany;
using Anemoi.Contract.Workspace.Commands.MemberCommands.UpdateActivatedMembers;
using Anemoi.Contract.Workspace.Errors;
using Anemoi.Contract.Workspace.ModelIds;
using AutoMapper;
using Serilog;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Cqrs.Commands.MemberCommands.UpdateActivatedMembers;

public sealed class UpdateActivatedMembersHandler(
    ISqlRepository<Member> sqlRepository,
    IWorkspaceIdGetter workspaceIdGetter,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger)
    : EfCommandManyVoidHandler<Member, UpdateActivatedMembersCommand>(sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandManyFlowBuilderVoid<Member> BuildCommand(IStartManyCommandVoid<Member> fromFlow,
        UpdateActivatedMembersCommand command, CancellationToken cancellationToken)
        => fromFlow
            .UpdateMany(x => command.Ids.Contains(x.Id) &&
                             x.WorkspaceId == new WorkspaceId(Guid.Parse(workspaceIdGetter.WorkspaceId)))
            .WithSpecialAction(null)
            .WithCondition(_ => None.Value)
            .WithModify(users => users.ForEach(u => { u.IsActivated = command.IsActivated; }))
            .WithErrorIfSaveChange(WorkspaceErrorDetail.MemberError.RemoveFailed());
}