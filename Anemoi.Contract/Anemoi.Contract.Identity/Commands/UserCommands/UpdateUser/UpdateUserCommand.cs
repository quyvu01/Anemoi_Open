using Anemoi.BuildingBlock.Application.Cqrs.Commands;

namespace Anemoi.Contract.Identity.Commands.UserCommands.UpdateUser;

public sealed record UpdateUserCommand(
    string FirstName,
    string LastName,
    string Avatar,
    string PhoneNumber) : ICommandVoid;