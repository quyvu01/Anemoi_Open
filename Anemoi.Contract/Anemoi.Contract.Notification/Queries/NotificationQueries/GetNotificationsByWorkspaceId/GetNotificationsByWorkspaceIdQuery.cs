using Anemoi.BuildingBlock.Application.Cqrs.Queries;

namespace Anemoi.Contract.Notification.Queries.NotificationQueries.GetNotificationsByWorkspaceId;

public record GetNotificationsByWorkspaceIdQuery(string WorkspaceId, string UserId) : IQueryCounting;