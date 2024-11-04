using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Infrastructure.PolicyHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Centralize.Infrastructure.Installers;

public sealed class AuthorizationInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAuthorizationHandler, HasOneOfPolicyHandler>();
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Internal", builder => builder.RequireClaim("applicationPolicyInternal", "Internal"));
            options.AddPolicy("Agency", builder => builder.RequireClaim("applicationPolicyAgency", "Agency"));
            options.AddPolicy("User", builder => builder.RequireClaim("applicationPolicyUser", "User"));
            options.AddPolicy("InternalOrAgency", builder => builder.Requirements
                .Add(new HasOneOfPolicyRequirement("Internal,Agency")));
        });
    }
}