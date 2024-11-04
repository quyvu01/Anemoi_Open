using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.Contract.Notification.Queries.EmailConfigurationQueries.GetDefaultEmailConfiguration;
using Anemoi.Notification.Application.Abstractions;
using Anemoi.Notification.Application.ApplicationModels;
using MediatR;

namespace Anemoi.Notification.Infrastructure.Services;

public class SmtpProvider(IWorkspaceIdGetter workspaceIdGetter, ISender sender) : ISmtpProvider
{
    public Task<SmtpConfiguration> SmtpConfiguration => GetDefaultSmtpConfiguration();

    private async Task<SmtpConfiguration> GetDefaultSmtpConfiguration()
    {
        var workspaceId = workspaceIdGetter.WorkspaceId ?? Guid.Empty.ToString();
        var defaultSmtpConfiguration = await sender
            .Send(new GetDefaultEmailConfigurationQuery(workspaceId));
        return defaultSmtpConfiguration
            .Match(e => new SmtpConfiguration(e.Name, e.UserName, e.Password, e.Host, e.Port), _ => null);
    }
}