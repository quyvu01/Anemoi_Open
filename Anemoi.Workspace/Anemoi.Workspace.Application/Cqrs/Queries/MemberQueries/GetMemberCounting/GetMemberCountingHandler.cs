using System.Linq.Expressions;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.CountingFlow;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryCounting;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Queries.MemberQueries.GetMemberCounting;
using Anemoi.Workspace.Domain.Models;
using Serilog;

namespace Anemoi.Workspace.Application.Cqrs.Queries.MemberQueries.GetMemberCounting;

public sealed class GetMemberCountingHandler(
    ISqlRepository<Member> sqlRepository,
    ILogger logger,
    IWorkspaceIdGetter workspaceIdGetter)
    : EfQueryCountingHandler<Member, GetMemberCountingQuery>(sqlRepository, logger)
{
    protected override ICountingFlowBuilder<Member> BuildQueryFlow(ICountingFilter<Member> fromFlow,
        GetMemberCountingQuery query)
    {
        Expression<Func<Member, bool>> activeFilter = query.IsActivated switch
        {
            { } val => m => m.IsActivated == val,
            _ => _ => true
        };
        Expression<Func<Member, bool>> removedFilter = query.IsRemoved switch
        {
            { } val => m => m.IsRemoved == val,
            _ => _ => true
        };
        var finalFilter = ExpressionHelper.CombineAnd(activeFilter, removedFilter,
            x => x.WorkspaceId == new WorkspaceId(Guid.Parse(workspaceIdGetter.WorkspaceId)));
        return fromFlow.WithFilter(finalFilter);
    }
}