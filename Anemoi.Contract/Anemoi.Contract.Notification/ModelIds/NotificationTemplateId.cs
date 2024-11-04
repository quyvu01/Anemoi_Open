using Anemoi.BuildingBlock.Domain;

namespace Anemoi.Contract.Notification.ModelIds;

public record NotificationTemplateId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public override string ToString() => Value.ToString();
}