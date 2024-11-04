using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.Contract.Notification.Responses;
using Newtonsoft.Json;

namespace Anemoi.Contract.Notification.Queries.EmailConfigurationQueries.GetEmailConfigurationsByWorkspace;

public record GetEmailConfigurationsByWorkspaceQuery : GetManyQuery, IQueryPaged<EmailConfigurationResponse>
{
    public string SearchKey { get; set; }
    [JsonIgnore] public string WorkspaceId { get; set; }
    public bool? IsDefault { get; set; }
}