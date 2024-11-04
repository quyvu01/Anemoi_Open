namespace Anemoi.Centralize.Application.Abstractions;

public interface ICustomUserIdSetter
{
    void SetUserId(string userId);
}

public interface ICustomUserIdGetter
{
    string UserId { get; }
}