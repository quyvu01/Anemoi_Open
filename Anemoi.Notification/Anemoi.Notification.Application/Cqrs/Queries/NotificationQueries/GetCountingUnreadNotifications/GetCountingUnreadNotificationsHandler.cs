using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.CountingFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryCounting;
using Anemoi.Contract.Notification.Queries.NotificationQueries.GetCountingUnreadNotifications;
using Serilog;

namespace Anemoi.Notification.Application.Cqrs.Queries.NotificationQueries.GetCountingUnreadNotifications;

public sealed class GetCountingUnreadNotificationsHandler(
    ISqlRepository<Anemoi.Notification.Domain.Models.Notification> sqlRepository,
    ILogger logger,
    IWorkspaceIdGetter workspaceIdGetter,
    IUserIdGetter userIdGetter) :
    EfQueryCountingHandler<Anemoi.Notification.Domain.Models.Notification, GetCountingUnreadNotificationsQuery>(
        sqlRepository, logger)
{
    protected override ICountingFlowBuilder<Anemoi.Notification.Domain.Models.Notification> BuildQueryFlow(
        ICountingFilter<Anemoi.Notification.Domain.Models.Notification> fromFlow,
        GetCountingUnreadNotificationsQuery query) =>
        fromFlow
            .WithFilter(x => x.WorkspaceId == workspaceIdGetter.WorkspaceId
                             && !x.IsRead && x.TargetUserId == userIdGetter.UserId);
}