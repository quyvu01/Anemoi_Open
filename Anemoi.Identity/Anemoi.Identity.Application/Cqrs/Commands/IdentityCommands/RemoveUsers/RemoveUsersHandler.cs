using System;
using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandMany;
using Anemoi.Contract.Identity.Commands.IdentityCommands.RemoveUsers;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.RemoveUsers;

public sealed class RemoveUsersHandler(
    IMapper mapper,
    ILogger logger,
    ISqlRepository<User> userDbRepository,
    IUnitOfWork unitOfWork)
    : EfCommandManyVoidHandler<User, RemoveUsersCommand>(userDbRepository, unitOfWork, mapper, logger)
{
    protected override ICommandManyFlowBuilderVoid<User> BuildCommand(
        IStartManyCommandVoid<User> fromFlow, RemoveUsersCommand command,
        CancellationToken cancellationToken) => fromFlow
        .UpdateMany(x => command.UserIds.Contains(x.UserId) && x.IsActivated)
        .WithSpecialAction(null)
        .WithCondition(users =>
        {
            if (users.Count == command.UserIds.Count) return None.Value;
            return IdentityErrorDetail.UserError.NotFound();
        })
        .WithModify(users =>
        {
            users.ForEach(u =>
            {
                u.IsRemoved = true;
                u.Email = $"{u.Email}_old_{DateTime.UtcNow.Ticks}";
                u.PhoneNumber = $"{u.PhoneNumber}_old_{DateTime.UtcNow.Ticks}";
            });
        })
        .WithErrorIfSaveChange(IdentityErrorDetail.UserError.RemoveFailed());
}