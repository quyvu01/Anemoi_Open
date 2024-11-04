using System.Collections.Generic;
using Newtonsoft.Json;

namespace Anemoi.Identity.Application.ApplicationModels.Firebase;

public class FirebaseAuthenticationResponse
{
    [JsonProperty("kind")] public string Kind { get; set; }

    [JsonProperty("users")] public List<FirebaseUserResponse> Users { get; set; }
}