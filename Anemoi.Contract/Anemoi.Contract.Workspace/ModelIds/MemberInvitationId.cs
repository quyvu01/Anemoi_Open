using Anemoi.BuildingBlock.Domain;

namespace Anemoi.Contract.Workspace.ModelIds;

public sealed record MemberInvitationId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public override string ToString() => base.ToString();
}