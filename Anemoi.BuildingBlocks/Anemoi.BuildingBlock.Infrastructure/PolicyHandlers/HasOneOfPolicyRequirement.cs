using Microsoft.AspNetCore.Authorization;

namespace Anemoi.BuildingBlock.Infrastructure.PolicyHandlers;

public sealed class HasOneOfPolicyRequirement(string policies) : IAuthorizationRequirement
{
    public string Policies { get; } = policies;
}