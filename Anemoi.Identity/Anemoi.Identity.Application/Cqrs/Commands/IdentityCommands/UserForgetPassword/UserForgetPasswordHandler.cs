using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.IdentityCommands.UserForgetPassword;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.UserForgetPassword;

public sealed class UserForgetPasswordHandler(
    ILogger logger,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    ISqlRepository<User> userDbRepository)
    : EfCommandOneVoidHandler<User, UserForgetPasswordCommand>(userDbRepository, unitOfWork, mapper, logger)
{

    protected override ICommandOneFlowBuilderVoid<User> BuildCommand(
        IStartOneCommandVoid<User> fromFlow,
        UserForgetPasswordCommand command, CancellationToken cancellationToken)
        => fromFlow
            .UpdateOne(x => x.Email == command.Email && x.IsActivated)
            .WithSpecialAction(x => x)
            .WithCondition(_ => None.Value)
            .WithModify(_ => { })
            .WithErrorIfNull(IdentityErrorDetail.UserError.NotFound())
            .WithErrorIfSaveChange(IdentityErrorDetail.UserError.UpdateFailed());
}