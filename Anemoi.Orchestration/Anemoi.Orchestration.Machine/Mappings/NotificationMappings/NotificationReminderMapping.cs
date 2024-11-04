using AutoMapper;
using Anemoi.Orchestration.Contract.NotificationContract.Events.NotificationReminderEvents;
using Anemoi.Orchestration.Contract.NotificationContract.Instances;

namespace Anemoi.Orchestrator.Machine.Mappings.NotificationMappings;

public class NotificationReminderMapping : Profile
{
    public NotificationReminderMapping()
    {
        CreateMap<CreateNotificationReminder, NotificationReminderInstance>();
    }
}