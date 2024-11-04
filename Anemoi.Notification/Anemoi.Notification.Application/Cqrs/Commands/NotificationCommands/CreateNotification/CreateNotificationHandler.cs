using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Notification.Commands.NotificationCommands.CreateNotification;
using Anemoi.Contract.Notification.Errors;
using AutoMapper;
using Serilog;

namespace Anemoi.Notification.Application.Cqrs.Commands.NotificationCommands.CreateNotification;

public sealed class CreateNotificationHandler(
    ISqlRepository<Anemoi.Notification.Domain.Models.Notification> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger) :
    EfCommandOneVoidHandler<Anemoi.Notification.Domain.Models.Notification, CreateNotificationCommand>(sqlRepository, unitOfWork, mapper,
        logger)
{
    protected override ICommandOneFlowBuilderVoid<Anemoi.Notification.Domain.Models.Notification> BuildCommand(
        IStartOneCommandVoid<Anemoi.Notification.Domain.Models.Notification> fromFlow, CreateNotificationCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .CreateOne(Mapper.Map<Anemoi.Notification.Domain.Models.Notification>(command))
            .WithCondition(_ => None.Value)
            .WithErrorIfSaveChange(NotificationErrorDetail.NotificationError.CreateFailed());
}