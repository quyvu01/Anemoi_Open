using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Anemoi.Identity.Application.ApplicationModels.Firebase;

public class FirebaseUserResponse
{
    [JsonProperty("localId")] public string LocalId { get; set; }

    [JsonProperty("email")] public string Email { get; set; }

    [JsonProperty("displayName")] public string DisplayName { get; set; }

    [JsonProperty("photoUrl")] public string PhotoUrl { get; set; }

    [JsonProperty("emailVerified")] public bool EmailVerified { get; set; }

    [JsonProperty("providerUserInfo")] public List<FirebaseProviderUserInfo> ProviderUserInfo { get; set; }

    [JsonProperty("validSince")] public string ValidSince { get; set; }

    [JsonProperty("lastLoginAt")] public string LastLoginAt { get; set; }

    [JsonProperty("createdAt")] public string CreatedAt { get; set; }

    [JsonProperty("lastRefreshAt")] public DateTime LastRefreshAt { get; set; }
}