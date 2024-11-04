using Anemoi.BuildingBlock.Application.Responses;

namespace Anemoi.Contract.Notification.Responses;

public sealed class EmailTemplateResponse : ModelResponse
{
    public string Type { get; set; }
    public string Content { get; set; }
    public string Subject { get; set; }
    public string Note { get; set; }
}