using Anemoi.BuildingBlock.Domain;
using Anemoi.Contract.Notification.ModelIds;

namespace Anemoi.Notification.Domain.Models;

public class NotificationTemplate : ValueObject
{
    public NotificationTemplateId Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield break;
    }
}