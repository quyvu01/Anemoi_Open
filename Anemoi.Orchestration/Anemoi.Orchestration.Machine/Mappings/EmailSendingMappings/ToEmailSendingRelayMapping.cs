using Anemoi.Orchestration.Contract.EmailSendingContract.Events.EmailSendingRelayEvents;
using Anemoi.Orchestration.Contract.EmailSendingContract.Instances;
using AutoMapper;

namespace Anemoi.Orchestrator.Machine.Mappings.EmailSendingMappings;

public class ToEmailSendingRelayMapping : Profile
{
    public ToEmailSendingRelayMapping()
    {
        CreateMap<CreateEmailSendingRelay, EmailSendingRelayInstance>();
    }
}