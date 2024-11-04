using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.ApplicationModels;

namespace Anemoi.BuildingBlock.Infrastructure.Services;

public class ApplicationPolicyService : IApplicationPolicyGetter, IApplicationPolicySetter
{
    public void SetApplicationPolicy(ApplicationPolicy applicationPolicy)
    {
        Key = applicationPolicy?.Key;
        Value = applicationPolicy?.Value;
    }

    public string Key { get; private set; }
    public string Value { get; private set; }
}