using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetWorkspacesByUser;

public sealed record GetWorkspacesByUserQuery : GetManyQuery, IQueryPaged<WorkspaceResponse>;