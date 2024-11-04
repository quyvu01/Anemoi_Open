using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.OrganizationQueries.GetOrganizations;

public sealed record GetOrganizationsQuery : GetManyQuery, IQueryPaged<OrganizationResponse>;