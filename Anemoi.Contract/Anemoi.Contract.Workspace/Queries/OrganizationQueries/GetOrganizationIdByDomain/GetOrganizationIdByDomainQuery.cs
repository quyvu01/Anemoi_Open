using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.OrganizationQueries.GetOrganizationIdByDomain;

public sealed record GetOrganizationIdByDomainQuery(string Domain) : IQueryOne<OrganizationIdResponse>;