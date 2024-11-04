using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Queries.EntityFramework.EfQueryOne;
using Anemoi.Contract.Workspace.Errors;
using Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetWorkspace;
using Anemoi.Contract.Workspace.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Serilog;

namespace Anemoi.Workspace.Application.Cqrs.Queries.WorkspaceQueries.GetWorkspace;

public sealed class GetWorkspaceHandler(
    ISqlRepository<Anemoi.Workspace.Domain.Models.Workspace> sqlRepository,
    IMapper mapper,
    ILogger logger)
    : EfQueryOneHandler<Anemoi.Workspace.Domain.Models.Workspace, GetWorkspaceQuery, WorkspaceResponse>(sqlRepository,
        mapper, logger)
{
    protected override IQueryOneFlowBuilder<Anemoi.Workspace.Domain.Models.Workspace, WorkspaceResponse> BuildQueryFlow(
        IQueryOneFilter<Anemoi.Workspace.Domain.Models.Workspace, WorkspaceResponse> fromFlow, GetWorkspaceQuery query)
        => fromFlow
            .WithFilter(x => x.Id == query.Id)
            .WithSpecialAction(x => x.ProjectTo<WorkspaceResponse>(Mapper.ConfigurationProvider))
            .WithErrorIfNull(WorkspaceErrorDetail.WorkspaceError.NotFound());
}