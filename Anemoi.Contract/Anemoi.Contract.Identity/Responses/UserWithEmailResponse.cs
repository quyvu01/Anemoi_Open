using Anemoi.BuildingBlock.Application.Responses;

namespace Anemoi.Contract.Identity.Responses;

public sealed class UserWithEmailResponse : ModelResponse
{
    public string Email { get; set; }
}