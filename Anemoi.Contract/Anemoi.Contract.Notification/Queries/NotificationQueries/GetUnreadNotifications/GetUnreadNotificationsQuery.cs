using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.Contract.Notification.Responses;

namespace Anemoi.Contract.Notification.Queries.NotificationQueries.GetUnreadNotifications;

public sealed record GetUnreadNotificationsQuery : GetManyQuery, IQueryPaged<NotificationResponse>;