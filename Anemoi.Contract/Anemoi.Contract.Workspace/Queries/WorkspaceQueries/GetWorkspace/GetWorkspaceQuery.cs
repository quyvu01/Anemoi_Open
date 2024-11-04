using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetWorkspace;

public sealed record GetWorkspaceQuery(WorkspaceId Id) : IQueryOne<WorkspaceResponse>;