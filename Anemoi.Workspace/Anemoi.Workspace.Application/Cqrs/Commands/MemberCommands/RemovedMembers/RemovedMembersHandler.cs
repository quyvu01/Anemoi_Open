using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandMany;
using Anemoi.Contract.Workspace.Commands.MemberCommands.RemoveMembers;
using Anemoi.Contract.Workspace.Errors;
using AutoMapper;
using Serilog;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Cqrs.Commands.MemberCommands.RemovedMembers;

public sealed class RemovedMembersHandler(
    ISqlRepository<Member> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger) : EfCommandManyVoidHandler<Member, RemoveMembersCommand>(sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandManyFlowBuilderVoid<Member> BuildCommand(IStartManyCommandVoid<Member> fromFlow,
        RemoveMembersCommand command, CancellationToken cancellationToken)
        => fromFlow
            .RemoveMany(x => command.Ids.Contains(x.Id))
            .WithSpecialAction(x => x)
            .WithCondition(_ => None.Value)
            .WithErrorIfSaveChange(WorkspaceErrorDetail.MemberError.RemoveFailed());
}