using AutoMapper;
using Anemoi.Orchestration.Contract.SecureContract.Events.OtpCreationEvents;
using Anemoi.Orchestration.Contract.SecureContract.Instances;

namespace Anemoi.Orchestrator.Machine.Mappings.SecureMappings;

public class ToOtpOperationMapping : Profile
{
    public ToOtpOperationMapping()
    {
        CreateMap<CreatePhoneNumberOtp, OtpOperationInstance>()
            .ForMember(a => a.LastRecordTime, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}