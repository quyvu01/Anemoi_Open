using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.Contract.Notification.Commands.EmailConfigurationCommands.CreateEmailConfiguration;
using Anemoi.Contract.Notification.Commands.EmailConfigurationCommands.UpdateEmailConfiguration;
using Anemoi.Contract.Notification.ModelIds;
using AutoMapper;
using Anemoi.Notification.Domain.Models;

namespace Anemoi.Notification.Application.Mappings.EmailConfigurationMappings;

public sealed class ToEmailConfigurationMapping : Profile
{
    public ToEmailConfigurationMapping()
    {
        CreateMap<CreateEmailConfigurationCommand, EmailConfiguration>()
            .ForMember(a => a.Id, opt => opt.MapFrom(_ => new EmailConfigurationId(IdGenerator.NextGuid())))
            .ForMember(a => a.IsSslEnabled, opt => opt.MapFrom(x => x.Port == 465))
            .ForMember(a => a.SearchHint, opt => opt.MapFrom(x => x.Name.GenerateSearchHint()))
            .ForMember(a => a.CreatedTime, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<UpdateEmailConfigurationCommand, EmailConfiguration>()
            .ForMember(a => a.Id, opt => opt.Ignore())
            .ForMember(a => a.Port, opt => opt.PreCondition(a => a.Port is { }))
            .ForMember(a => a.SearchHint, opt =>
            {
                opt.PreCondition(a => a.Name is { });
                opt.MapFrom(x => x.Name.GenerateSearchHint());
            })
            .ForMember(a => a.IsSslEnabled, opt =>
            {
                opt.PreCondition(a => a.Port is { });
                opt.MapFrom(a => a.Port == 465);
            })
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember is { }));
        ;
    }
}