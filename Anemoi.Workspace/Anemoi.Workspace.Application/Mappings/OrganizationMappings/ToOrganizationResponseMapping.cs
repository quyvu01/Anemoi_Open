using Anemoi.Contract.Workspace.Responses;
using AutoMapper;
using Anemoi.Workspace.Domain.Models;

namespace Anemoi.Workspace.Application.Mappings.OrganizationMappings;

public class ToOrganizationResponseMapping : Profile
{
    public ToOrganizationResponseMapping()
    {
        CreateMap<Organization, OrganizationResponse>()
            .ForMember(a => a.ParentOrganizationId,
                opt => opt.MapFrom(x => x.ParentOrganizationId == null ? null : x.ParentOrganizationId.ToString()))
            .ForMember(x => x.MemberQuantity, opt => opt.MapFrom(x => x.MemberMapOrganizations.Count));
    }
}