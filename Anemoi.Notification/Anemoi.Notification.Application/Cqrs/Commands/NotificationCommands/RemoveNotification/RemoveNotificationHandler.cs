using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandMany;
using Anemoi.Contract.Notification.Commands.NotificationCommands.RemoveNotification;
using Anemoi.Contract.Notification.Errors;
using AutoMapper;
using Serilog;

namespace Anemoi.Notification.Application.Cqrs.Commands.NotificationCommands.RemoveNotification;

public sealed class RemoveNotificationHandler(
    ISqlRepository<Anemoi.Notification.Domain.Models.Notification> sqlRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger logger) :
    EfCommandManyVoidHandler<Anemoi.Notification.Domain.Models.Notification, RemoveNotificationCommand>(sqlRepository,
        unitOfWork, mapper, logger)
{
    protected override ICommandManyFlowBuilderVoid<Anemoi.Notification.Domain.Models.Notification> BuildCommand(
        IStartManyCommandVoid<Anemoi.Notification.Domain.Models.Notification> fromFlow,
        RemoveNotificationCommand command,
        CancellationToken cancellationToken)
        => fromFlow
            .UpdateMany(x => command.NotificationIds.Contains(x.Id))
            .WithSpecialAction(null)
            .WithCondition(_ => None.Value)
            .WithModify(x => x.ForEach(a => a.IsTrash = true))
            .WithErrorIfSaveChange(NotificationErrorDetail.NotificationError.RemoveFailed());
}