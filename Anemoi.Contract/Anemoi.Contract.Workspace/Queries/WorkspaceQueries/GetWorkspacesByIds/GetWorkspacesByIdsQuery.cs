using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetWorkspacesByIds;

public sealed record GetWorkspacesByIdsQuery(List<WorkspaceId> Ids) : IQueryCollection<WorkspaceResponse>;