using Anemoi.Centralize.Application.Abstractions;

namespace Anemoi.Centralize.Infrastructure.Services;

public class CustomUserIdService : ICustomUserIdGetter, ICustomUserIdSetter
{
    public string UserId { get; private set; }
    public void SetUserId(string userId) => UserId = userId;
}