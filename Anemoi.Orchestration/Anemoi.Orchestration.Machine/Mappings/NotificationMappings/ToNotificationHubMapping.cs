using AutoMapper;
using Anemoi.Orchestration.Contract.NotificationContract.Events.NotificationHubEvents;
using Anemoi.Orchestration.Contract.NotificationContract.Instances;

namespace Anemoi.Orchestrator.Machine.Mappings.NotificationMappings;

public sealed class ToNotificationHubMapping : Profile
{
    public ToNotificationHubMapping()
    {
        CreateMap<CreateNotification, NotificationHubInstance>();
    }
}