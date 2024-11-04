using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.ModelIds;
using Newtonsoft.Json;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.LockUser;

public sealed record LockUserCommand
    ([property: JsonIgnore] UserId UserId, bool EnableLock, DateTime? LockUntil) : ICommandVoid;