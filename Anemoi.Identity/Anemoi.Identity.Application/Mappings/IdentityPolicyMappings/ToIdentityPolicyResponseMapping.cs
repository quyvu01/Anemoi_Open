using System.Linq;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Domain.Models;
using AutoMapper;

namespace Anemoi.Identity.Application.Mappings.IdentityPolicyMappings;

public class ToIdentityPolicyResponseMapping : Profile
{
    public ToIdentityPolicyResponseMapping()
    {
        CreateMap<IdentityPolicy, IdentityPolicyResponse>()
            .ForMember(x => x.UserRoles,
                opt => opt.MapFrom(x => x.IdentityPolicyMapRoles
                    .Select(r => r.Role).OrderBy(r => r.Name)));
    }
}