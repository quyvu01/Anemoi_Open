using Anemoi.BuildingBlock.Application.Responses;

namespace Anemoi.Contract.Notification.Responses;

public sealed class NotificationResponse : ModelResponse
{
    public string CorrelationId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public string CreatorId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; }
    public string Parameters { get; set; }
}