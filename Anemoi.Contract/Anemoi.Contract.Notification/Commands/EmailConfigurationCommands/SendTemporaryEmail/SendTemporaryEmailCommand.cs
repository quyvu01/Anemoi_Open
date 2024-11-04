using Anemoi.BuildingBlock.Application.Cqrs.Commands;

namespace Anemoi.Contract.Notification.Commands.EmailConfigurationCommands.SendTemporaryEmail;

public sealed record SendTemporaryEmailCommand : ICommandVoid
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string EmailTo { get; set; }
}