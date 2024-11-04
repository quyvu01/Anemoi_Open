using System;
using System.Text.Json;
using System.Threading.Tasks;
using MassTransit;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.ApplicationModels;

namespace Anemoi.BuildingBlock.Infrastructure.Filters;

public sealed class RequestHeaderFilter<T>(
    IUserIdSetter userIdSetter,
    IWorkspaceIdSetter workspaceIdSetter,
    IAdministratorSetter administratorSetter,
    IApplicationPolicySetter applicationPolicySetter,
    ITokenSetter tokenSetter)
    : IFilter<ConsumeContext<T>>
    where T : class
{
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        if (context.Headers.TryGetHeader("userId", out var userId))
            userIdSetter.SetUserId(userId?.ToString());
        if (context.Headers.TryGetHeader("workspaceId", out var workspaceId))
            workspaceIdSetter.SetWorkspaceId(workspaceId?.ToString());
        if (context.Headers.TryGetHeader("administrator", out var isAdministrator))
            administratorSetter.SetAdministrator(isAdministrator?.ToString() == $"{true}");
        if (context.Headers.TryGetHeader("token", out var token))
            tokenSetter.SetToken(token?.ToString());
        if (context.Headers.TryGetHeader("applicationPolicy", out var applicationPolicy))
        {
            try
            {
                var policy = JsonSerializer.Deserialize<ApplicationPolicy>(applicationPolicy.ToString()!);
                applicationPolicySetter.SetApplicationPolicy(policy);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
    }
}