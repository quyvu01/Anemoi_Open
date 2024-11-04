using Newtonsoft.Json;

namespace Anemoi.Identity.Application.ApplicationModels.Apple;

public sealed class AppleUserData
{
    [JsonProperty("iss")] public string Iss { get; set; }
    [JsonProperty("aud")] public string Aud { get; set; }
    [JsonProperty("email")] public string Email { get; set; }
    [JsonProperty("sub")] public string Sub { get; set; }
}