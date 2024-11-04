using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.Contract.Notification.Responses;

namespace Anemoi.Contract.Notification.Queries.EmailTemplateQueries.GetContentEmailTemplateByType;

public sealed record GetContentEmailTemplateByTypeQuery(string Type, object Values) : IQueryOne<EmailTemplateResponse>;