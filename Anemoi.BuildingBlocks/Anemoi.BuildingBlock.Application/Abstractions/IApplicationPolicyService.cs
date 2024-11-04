using Anemoi.BuildingBlock.Application.ApplicationModels;

namespace Anemoi.BuildingBlock.Application.Abstractions;

public interface IApplicationPolicyGetter
{
    string Key { get; }
    string Value { get; }
}

public interface IApplicationPolicySetter
{
    void SetApplicationPolicy(ApplicationPolicy applicationPolicy);
}