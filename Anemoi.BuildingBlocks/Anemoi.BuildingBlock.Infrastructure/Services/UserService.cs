using Anemoi.BuildingBlock.Application.Abstractions;

namespace Anemoi.BuildingBlock.Infrastructure.Services;

public class UserService : IUserIdGetter, IUserIdSetter
{
    public string UserId { get; private set; }

    public void SetUserId(string userId) => UserId = userId;
}