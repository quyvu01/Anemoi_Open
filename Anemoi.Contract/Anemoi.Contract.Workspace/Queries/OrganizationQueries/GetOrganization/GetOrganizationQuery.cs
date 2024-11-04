using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Workspace.ModelIds;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.OrganizationQueries.GetOrganization;

public sealed record GetOrganizationQuery(OrganizationId Id) : IQueryOne<OrganizationResponse>;