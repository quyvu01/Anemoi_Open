using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Contract.Identity.Responses;
using Newtonsoft.Json;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.ChangePasswordWithoutOldPassword;

public sealed record ChangePasswordWithoutOldPasswordCommand
    ([property: JsonIgnore] UserId UserId, string NewPassword) :
        ICommandResult<UserIdResponse>;