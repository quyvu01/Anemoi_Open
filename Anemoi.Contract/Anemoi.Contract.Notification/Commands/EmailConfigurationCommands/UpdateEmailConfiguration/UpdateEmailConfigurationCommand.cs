using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Notification.ModelIds;
using Newtonsoft.Json;

namespace Anemoi.Contract.Notification.Commands.EmailConfigurationCommands.UpdateEmailConfiguration;

public sealed record UpdateEmailConfigurationCommand([property: JsonIgnore] EmailConfigurationId Id) : ICommandVoid
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int? Port { get; set; }
    public string Name { get; set; }
    public bool? IsDefault { get; set; }
}