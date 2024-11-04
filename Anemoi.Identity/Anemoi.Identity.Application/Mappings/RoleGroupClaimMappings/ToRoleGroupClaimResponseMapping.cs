using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Domain.Models;
using AutoMapper;

namespace Anemoi.Identity.Application.Mappings.RoleGroupClaimMappings;

public class ToRoleGroupClaimResponseMapping : Profile
{
    public ToRoleGroupClaimResponseMapping()
    {
        CreateMap<RoleGroupClaim, RoleGroupClaimResponse>();
    }
}