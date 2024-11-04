using Anemoi.BuildingBlock.Domain;

namespace Anemoi.Contract.Identity.ModelIds;

public sealed record RoleGroupMapUserRoleId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public override string ToString() => base.ToString();
}