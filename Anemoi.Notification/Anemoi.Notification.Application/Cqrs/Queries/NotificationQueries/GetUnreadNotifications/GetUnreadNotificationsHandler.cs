using System.Linq.Expressions;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Notification.Queries.NotificationQueries.GetUnreadNotifications;
using Anemoi.Contract.Notification.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;

namespace Anemoi.Notification.Application.Cqrs.Queries.NotificationQueries.GetUnreadNotifications;

public sealed class GetUnreadNotificationsHandler(
    ISqlRepository<Anemoi.Notification.Domain.Models.Notification> sqlRepository,
    IMapper mapper,
    ILogger logger,
    IWorkspaceIdGetter workspaceIdGetter,
    IUserIdGetter userIdGetter) :
    EfQueryPaginationHandler<Anemoi.Notification.Domain.Models.Notification, GetUnreadNotificationsQuery,
        NotificationResponse>(
        sqlRepository, mapper, logger)
{
    protected override IQueryListFlowBuilder<Anemoi.Notification.Domain.Models.Notification, NotificationResponse>
        BuildQueryFlow(
            IQueryListFilter<Anemoi.Notification.Domain.Models.Notification, NotificationResponse> fromFlow,
            GetUnreadNotificationsQuery query)
        => fromFlow
            .WithFilter(BuildFilter().And(x => !x.IsRead && !x.IsTrash))
            .WithSpecialAction(x => x.ProjectTo<NotificationResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(x => x.CreatedTime)
            .WithSortedDirectionWhenNotSet(SortedDirection.Descending);

    private Expression<Func<Anemoi.Notification.Domain.Models.Notification, bool>> BuildFilter()
    {
        Expression<Func<Anemoi.Notification.Domain.Models.Notification, bool>> filter =
            (workspaceIdGetter.WorkspaceId, userIdGetter.UserId) switch
            {
                ({ } workspaceId, { } userId) => x => x.WorkspaceId == workspaceId && x.TargetUserId == userId,
                (null, { } userId) => x => x.TargetUserId == userId,
                _ => _ => false
            };
        return ExpressionHelper.CombineAnd(filter);
    }
}