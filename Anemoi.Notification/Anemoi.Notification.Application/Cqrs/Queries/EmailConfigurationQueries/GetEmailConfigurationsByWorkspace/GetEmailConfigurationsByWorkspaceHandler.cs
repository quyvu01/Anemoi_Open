using System.Linq.Expressions;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryMany;
using Anemoi.Contract.Notification.Queries.EmailConfigurationQueries.GetEmailConfigurationsByWorkspace;
using Anemoi.Contract.Notification.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;
using Anemoi.Notification.Domain.Models;

namespace Anemoi.Notification.Application.Cqrs.Queries.EmailConfigurationQueries.GetEmailConfigurationsByWorkspace;

public sealed class GetEmailConfigurationsByWorkspaceHandler(
    ISqlRepository<EmailConfiguration> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryPaginationHandler<EmailConfiguration, GetEmailConfigurationsByWorkspaceQuery, EmailConfigurationResponse>(
        sqlRepository, mapper, logger)
{
    protected override IQueryListFlowBuilder<EmailConfiguration, EmailConfigurationResponse> BuildQueryFlow(
        IQueryListFilter<EmailConfiguration, EmailConfigurationResponse> fromFlow,
        GetEmailConfigurationsByWorkspaceQuery query)
    {
        Expression<Func<EmailConfiguration, bool>> searchKeyFilter = query.SearchKey switch
        {
            { } val => r => r.SearchHint.Contains(val.GenerateSearchHint()),
            _ => _ => true
        };
        Expression<Func<EmailConfiguration, bool>> emailFilter = query.SearchKey switch
        {
            { } val => r => r.UserName.Contains(val),
            _ => _ => true
        };
        Expression<Func<EmailConfiguration, bool>> defaultFilter = query.IsDefault switch
        {
            { } val => r => r.IsDefault == val,
            _ => _ => true
        };
        Expression<Func<EmailConfiguration, bool>> workspaceFilter = query.WorkspaceId switch
        {
            { } val => r => r.WorkspaceId == val,
            _ => _ => true
        };

        var finalFilter = searchKeyFilter.Or(emailFilter).And(defaultFilter).And(workspaceFilter);
        return fromFlow
            .WithFilter(finalFilter)
            .WithSpecialAction(r => r.ProjectTo<EmailConfigurationResponse>(Mapper.ConfigurationProvider))
            .WithSortFieldWhenNotSet(x => x.CreatedTime)
            .WithSortedDirectionWhenNotSet(SortedDirection.Descending);
    }
}