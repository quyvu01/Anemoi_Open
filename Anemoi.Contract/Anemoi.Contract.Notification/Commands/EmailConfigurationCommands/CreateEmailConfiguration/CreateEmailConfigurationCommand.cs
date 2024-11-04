using Anemoi.BuildingBlock.Application.Cqrs.Commands;

namespace Anemoi.Contract.Notification.Commands.EmailConfigurationCommands.CreateEmailConfiguration;

public sealed record CreateEmailConfigurationCommand : ICommandVoid
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string Name { get; set; }
    public bool IsDefault { get; set; }
}