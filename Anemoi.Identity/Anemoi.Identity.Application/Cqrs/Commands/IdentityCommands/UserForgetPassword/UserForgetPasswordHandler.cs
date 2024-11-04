using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.IdentityCommands.UserForgetPassword;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Domain.Models;
using Anemoi.Orchestration.Contract.EmailSendingContract.Events.EmailSendingRelayEvents;
using AutoMapper;
using MassTransit;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.UserForgetPassword;

public sealed class UserForgetPasswordHandler(
    ILogger logger,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IPublishEndpoint publishEndpoint,
    ISqlRepository<User> userDbRepository)
    : EfCommandOneVoidHandler<User, UserForgetPasswordCommand>(userDbRepository, unitOfWork, mapper, logger)
{
    protected override async Task AfterSaveChangesAsync(UserForgetPasswordCommand command,
        User model, CancellationToken cancellationToken)
    {
        var token = await userRepository.GenerateEmailResetPassTokenAsync(model);
        await publishEndpoint.Publish<CreateEmailSendingRelay>(new
        {
            CorrelationId = model.UserId.Value, Router = "userForgetPassword", EmailTo = model.Email,
            Parameters = new Dictionary<string, string>
            {
                { nameof(model.FirstName), model.FirstName },
                { nameof(model.LastName), model.LastName },
                { "Code", token }
            }
        }, cancellationToken);
    }

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