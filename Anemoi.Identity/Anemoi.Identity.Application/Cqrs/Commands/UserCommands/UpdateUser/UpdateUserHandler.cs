using System;
using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.UserCommands.UpdateUser;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Commands.UserCommands.UpdateUser;

public sealed class UpdateUserHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger,
    ISqlRepository<User> userDbRepository,
    IUserIdGetter userIdGetter)
    : EfCommandOneVoidHandler<User, UpdateUserCommand>(userDbRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderVoid<User> BuildCommand(
        IStartOneCommandVoid<User> fromFlow, UpdateUserCommand command,
        CancellationToken cancellationToken) => fromFlow
        .UpdateOne(x => x.UserId == new UserId(Guid.Parse(userIdGetter.UserId)) && x.IsActivated)
        .WithSpecialAction(null)
        .WithCondition(_ => None.Value)
        .WithModify(user => Mapper.Map(command, user))
        .WithErrorIfNull(IdentityErrorDetail.UserError.NotFound())
        .WithErrorIfSaveChange(IdentityErrorDetail.UserError.UpdateFailed());
}