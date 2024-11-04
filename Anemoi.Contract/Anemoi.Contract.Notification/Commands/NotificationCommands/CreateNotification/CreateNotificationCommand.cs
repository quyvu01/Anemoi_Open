using Anemoi.BuildingBlock.Application.Cqrs.Commands;

namespace Anemoi.Contract.Notification.Commands.NotificationCommands.CreateNotification;

public record CreateNotificationCommand : ICommandVoid
{
    public string WorkspaceId { get; set; }
    public string TargetUserId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public string Title { get; set; }
    public string CreatorId { get; set; }
    public string Content { get; set; }
    public string Parameters { get; set; }
}