using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetWorkspaces;

public sealed record GetWorkspacesQuery(string SearchKey, string UserId) :
    GetManyQuery, IQueryPaged<WorkspaceResponse>;