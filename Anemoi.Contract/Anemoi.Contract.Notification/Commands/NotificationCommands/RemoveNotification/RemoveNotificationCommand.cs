using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Notification.ModelIds;

namespace Anemoi.Contract.Notification.Commands.NotificationCommands.RemoveNotification;

public record RemoveNotificationCommand(List<NotificationId> NotificationIds) : ICommandVoid;