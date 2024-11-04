using Anemoi.Contract.Notification.Responses;
using AutoMapper;

namespace Anemoi.Notification.Application.Mappings.NotificationMappings;

public sealed class ToNotificationResponseMapping : Profile
{
    public ToNotificationResponseMapping()
    {
        CreateMap<Anemoi.Notification.Domain.Models.Notification, NotificationResponse>();
    }
}