﻿using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.Contract.Notification.Responses;

namespace Anemoi.Contract.Notification.Queries.NotificationQueries.GetNotifications;

public sealed record GetNotificationsQuery(bool ExcludeUnread) : GetManyQuery, IQueryPaged<NotificationResponse>;