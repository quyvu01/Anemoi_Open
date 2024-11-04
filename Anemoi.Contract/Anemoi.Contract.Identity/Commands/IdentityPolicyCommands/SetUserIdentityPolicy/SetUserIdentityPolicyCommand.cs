using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.ModelIds;

namespace Anemoi.Contract.Identity.Commands.IdentityPolicyCommands.SetUserIdentityPolicy;

public sealed record SetUserIdentityPolicyCommand(UserId UserId, IdentityPolicyId IdentityPolicyId) : ICommandVoid;