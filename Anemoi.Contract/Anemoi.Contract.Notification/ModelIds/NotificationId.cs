using Anemoi.BuildingBlock.Domain;

namespace Anemoi.Contract.Notification.ModelIds;

public sealed record NotificationId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public override string ToString() => Value.ToString();
}