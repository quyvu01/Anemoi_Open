using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.Contract.Notification.Commands.NotificationCommands.CreateNotification;
using Anemoi.Contract.Notification.ModelIds;
using AutoMapper;

namespace Anemoi.Notification.Application.Mappings.NotificationMappings;

public sealed class ToNotificationMapping : Profile
{
    public ToNotificationMapping()
    {
        CreateMap<CreateNotificationCommand, Anemoi.Notification.Domain.Models.Notification>()
            .ForMember(x => x.Id, opt => opt.MapFrom(_ => new NotificationId(IdGenerator.NextGuid())))
            .ForMember(x => x.CreatedTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(x => x.StartTime, opt => opt.MapFrom(_ => DateTime.UtcNow));
        // CreateMap<MarkAsReadNotificationCommand, Domain.Models.Notification>()
        //     .ForMember(x => x.IsRead, opt => opt.MapFrom(_ => true));
        // CreateMap<RemoveNotificationCommand, Domain.Models.Notification>()
        //     .ForMember(x => x.IsTrash, opt => opt.MapFrom(_ => true));
    }
}