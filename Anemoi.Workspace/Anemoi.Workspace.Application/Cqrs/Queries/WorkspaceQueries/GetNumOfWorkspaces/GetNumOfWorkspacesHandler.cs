using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.CountingFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryCounting;
using Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetNumOfWorkspaces;
using Serilog;

namespace Anemoi.Workspace.Application.Cqrs.Queries.WorkspaceQueries.GetNumOfWorkspaces;

public sealed class GetNumOfWorkspacesHandler(
    ISqlRepository<Domain.Models.Workspace> sqlRepository,
    IUserIdGetter userIdGetter,
    ILogger logger)
    : EfQueryCountingHandler<Domain.Models.Workspace, GetNumOfWorkspacesQuery>(sqlRepository, logger)
{
    protected override ICountingFlowBuilder<Domain.Models.Workspace> BuildQueryFlow(
        ICountingFilter<Domain.Models.Workspace> fromFlow, GetNumOfWorkspacesQuery query)
        => fromFlow
            .WithFilter(x => x.UserId == userIdGetter.UserId && !x.IsRemoved);
}