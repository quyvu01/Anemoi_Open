using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Anemoi.BuildingBlock.Infrastructure.PolicyHandlers;

public sealed class HasOneOfPolicyHandler(IAuthorizationPolicyProvider policyProvider)
    : AuthorizationHandler<HasOneOfPolicyRequirement>
{
    private static readonly ConcurrentDictionary<string, Lazy<AuthorizationPolicy>> PoliciesStorage = new();

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        HasOneOfPolicyRequirement requirement)
    {
        var requiredPolicies = requirement.Policies?.Split(",");
        if (requiredPolicies is null) return;
        foreach (var requiredPolicy in requiredPolicies)
        {
            var policy = await GetOrAddNewPolicyAsync(requiredPolicy);
            if (policy is null) return;
            var requirements = policy.Requirements;
            var claimsRequirements = requirements.OfType<ClaimsAuthorizationRequirement>().ToList();
            var hasClaim = context.User.Claims.Any(x =>
            {
                var matchClaim = claimsRequirements
                    .FirstOrDefault(c => c.ClaimType == x.Type);
                var allowedValues = matchClaim?.AllowedValues;
                return allowedValues?.Contains(x.Value) ?? false;
            });
            if (!hasClaim) continue;
            context.Succeed(requirement);
            return;
        }
    }

    private async Task<AuthorizationPolicy> GetOrAddNewPolicyAsync(string requiredPolicy)
    {
        if (PoliciesStorage.TryGetValue(requiredPolicy, out var policyLazy)) return policyLazy.Value;
        var authorizationPolicy = await policyProvider.GetPolicyAsync(requiredPolicy);
        PoliciesStorage.TryAdd(requiredPolicy, new Lazy<AuthorizationPolicy>(() => authorizationPolicy));
        return authorizationPolicy;
    }
}