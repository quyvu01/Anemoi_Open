using Anemoi.BuildingBlock.Domain;

namespace Anemoi.Contract.MasterData.ModelIds;

public record ProvinceId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public override string ToString() => base.ToString();
}