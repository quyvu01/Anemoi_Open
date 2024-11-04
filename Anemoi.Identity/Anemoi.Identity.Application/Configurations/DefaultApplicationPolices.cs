using System.Collections.Generic;

namespace Anemoi.Identity.Application.Configurations;

public class DefaultApplicationPolices
{
    public List<ApplicationPolicyData> ApplicationPolicies { get; set; }
}

public class ApplicationPolicyData
{
    public string Key { get; set; }
    public string Value { get; set; }
    public List<string> Roles { get; set; }
}