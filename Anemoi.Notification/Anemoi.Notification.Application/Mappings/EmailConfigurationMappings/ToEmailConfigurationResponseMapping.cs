using Anemoi.Contract.Notification.Responses;
using AutoMapper;
using Anemoi.Notification.Domain.Models;

namespace Anemoi.Notification.Application.Mappings.EmailConfigurationMappings;

public sealed class ToEmailConfigurationResponseMapping : Profile
{
    public ToEmailConfigurationResponseMapping()
    {
        CreateMap<EmailConfiguration, EmailConfigurationResponse>();
    }
}