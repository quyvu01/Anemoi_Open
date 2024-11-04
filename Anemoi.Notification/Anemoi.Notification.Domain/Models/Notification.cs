using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Notification.ModelIds;

namespace Anemoi.Notification.Domain.Models;

public class Notification : ValueObject
{
    public NotificationId Id { get; set; }
    public string WorkspaceId { get; set; }
    public string TargetUserId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public string CreatorId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; }
    public bool IsTrash { get; set; }
    public string Parameters { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}