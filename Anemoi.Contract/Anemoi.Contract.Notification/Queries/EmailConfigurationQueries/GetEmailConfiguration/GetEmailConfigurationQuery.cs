using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Notification.ModelIds;
using Anemoi.Contract.Notification.Responses;

namespace Anemoi.Contract.Notification.Queries.EmailConfigurationQueries.GetEmailConfiguration;

public sealed record GetEmailConfigurationQuery(EmailConfigurationId Id) : IQueryOne<EmailConfigurationResponse>;