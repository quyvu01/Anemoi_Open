using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Workspace.Responses;

namespace Anemoi.Contract.Workspace.Queries.OrganizationQueries.CheckIfSubDomainIsValid;

public sealed record CheckIfSubDomainIsValidQuery(string SubDomain) : IQueryOne<OrganizationIdResponse>;