using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.WorkspaceQueries.GetAllWorkspaceIds;

public sealed record GetAllWorkspaceIdsQuery : IQueryCollection<WorkspaceIdResponse>;