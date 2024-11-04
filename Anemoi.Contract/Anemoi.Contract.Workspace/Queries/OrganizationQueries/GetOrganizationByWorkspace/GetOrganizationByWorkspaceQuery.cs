using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.OrganizationQueries.GetOrganizationByWorkspace;

public sealed record GetOrganizationByWorkspaceQuery(WorkspaceId WorkspaceId) : IQueryOne<OrganizationResponse>;