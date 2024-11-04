using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Notification.Responses;

namespace Anemoi.Contract.Notification.Queries.EmailConfigurationQueries.GetDefaultEmailConfiguration;

public sealed record GetDefaultEmailConfigurationQuery(string WorkspaceId): IQueryOne<EmailConfigurationResponse>;