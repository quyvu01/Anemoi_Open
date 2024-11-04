using Newtonsoft.Json;

namespace Anemoi.Identity.Application.ApplicationModels.Firebase;

public class FirebaseProviderUserInfo
{
    [JsonProperty("providerId")] public string ProviderId { get; set; }

    [JsonProperty("displayName")] public string DisplayName { get; set; }

    [JsonProperty("photoUrl")] public string PhotoUrl { get; set; }

    [JsonProperty("federatedId")] public string FederatedId { get; set; }

    [JsonProperty("email")] public string Email { get; set; }

    [JsonProperty("rawId")] public string RawId { get; set; }
}