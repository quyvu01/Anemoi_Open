using Anemoi.BuildingBlock.Application.Responses;

namespace Anemoi.Contract.Identity.Responses;

public sealed class UserResponse : ModelResponse
{
    public string Email { get; set; }
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string PhoneNumber { get; set; }
    public string Avatar { get; set; }
    public DateTime CreatedTime { get; set; }
}