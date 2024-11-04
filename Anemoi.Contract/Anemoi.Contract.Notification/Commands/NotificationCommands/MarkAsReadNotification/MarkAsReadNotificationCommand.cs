using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Notification.ModelIds;

namespace Anemoi.Contract.Notification.Commands.NotificationCommands.MarkAsReadNotification;

public record MarkAsReadNotificationCommand(List<NotificationId> NotificationIds) : ICommandVoid;