using Anemoi.BuildingBlock.Domain;

namespace Anemoi.Contract.Workspace.ModelIds;

public sealed record OrganizationId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public override string ToString() => base.ToString();
}