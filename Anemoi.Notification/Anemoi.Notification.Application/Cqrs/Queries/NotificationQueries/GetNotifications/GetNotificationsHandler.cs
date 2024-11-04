using System.Linq.Expressions;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Notification.Queries.NotificationQueries.GetNotifications;
using Anemoi.Contract.Notification.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;

namespace Anemoi.Notification.Application.Cqrs.Queries.NotificationQueries.GetNotifications;

public sealed class GetNotificationsHandler(
    ISqlRepository<Domain.Models.Notification> sqlRepository,
    IWorkspaceIdGetter workspaceIdGetter,
    IMapper mapper,
    ILogger logger,
    IUserIdGetter userIdGetter) :
    EfQueryPaginationHandler<Domain.Models.Notification, GetNotificationsQuery,
        NotificationResponse>(sqlRepository,
        mapper, logger)
{
    protected override IQueryListFlowBuilder<Domain.Models.Notification, NotificationResponse>
        BuildQueryFlow(
            IQueryListFilter<Domain.Models.Notification, NotificationResponse> fromFlow, GetNotificationsQuery query)
    {
        Expression<Func<Anemoi.Notification.Domain.Models.Notification, bool>> filter =
            (workspaceIdGetter.WorkspaceId, userIdGetter.UserId) switch
            {
                ({ } workspaceId, { } userId) => x => x.WorkspaceId == workspaceId && x.TargetUserId == userId,
                (null, { } userId) => x => x.TargetUserId == userId,
                _ => _ => false
            };
        Expression<Func<Anemoi.Notification.Domain.Models.Notification, bool>> excludeFilter =
            query.ExcludeUnread switch
            {
                true => n => !n.IsRead,
                _ => _ => false
            };
        return fromFlow
            .WithFilter(filter.And(excludeFilter).And(x => !x.IsTrash))
            .WithSpecialAction(x => x.ProjectTo<NotificationResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(x => x.CreatedTime)
            .WithSortedDirectionWhenNotSet(SortedDirection.Descending);
    }
}