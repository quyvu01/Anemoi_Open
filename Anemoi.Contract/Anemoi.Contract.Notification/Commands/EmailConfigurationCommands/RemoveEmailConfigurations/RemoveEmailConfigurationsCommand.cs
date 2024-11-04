using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Notification.ModelIds;

namespace Anemoi.Contract.Notification.Commands.EmailConfigurationCommands.RemoveEmailConfigurations;

public sealed record RemoveEmailConfigurationsCommand(List<EmailConfigurationId> Ids) : ICommandVoid;