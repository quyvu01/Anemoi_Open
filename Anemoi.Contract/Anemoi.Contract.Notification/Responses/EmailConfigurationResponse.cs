using Anemoi.BuildingBlock.Application.Responses;
using Newtonsoft.Json;

namespace Anemoi.Contract.Notification.Responses;

public sealed class EmailConfigurationResponse : ModelResponse
{
    public string UserName { get; set; }

    [property: System.Text.Json.Serialization.JsonIgnore]
    [property: JsonIgnore]
    public byte[] PasswordBytes { get; set; }

    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public bool IsSslEnabled { get; set; }
    public bool IsDefault { get; set; }
    public string Name { get; set; }
}