using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.CountingFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryCounting;
using Anemoi.Contract.Notification.Queries.NotificationQueries.GetNotificationsByWorkspaceId;
using Serilog;

namespace Anemoi.Notification.Application.Cqrs.Queries.NotificationQueries.GetNotificationsByWorkspaceId;

public sealed class GetNotificationsByWorkspaceIdHandler(
    ISqlRepository<Anemoi.Notification.Domain.Models.Notification> sqlRepository,
    ILogger logger) :
    EfQueryCountingHandler<Anemoi.Notification.Domain.Models.Notification, GetNotificationsByWorkspaceIdQuery>(
        sqlRepository, logger)
{
    protected override ICountingFlowBuilder<Anemoi.Notification.Domain.Models.Notification> BuildQueryFlow(
        ICountingFilter<Anemoi.Notification.Domain.Models.Notification> fromFlow,
        GetNotificationsByWorkspaceIdQuery query) =>
        fromFlow
            .WithFilter(x => x.WorkspaceId == query.WorkspaceId
                             && x.TargetUserId == query.UserId && !x.IsRead);
}