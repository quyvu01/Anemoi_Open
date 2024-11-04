namespace Anemoi.BuildingBlock.Application.Abstractions;

public interface IUserIdGetter
{
    string UserId { get; }
}
public interface IUserIdSetter
{
    void SetUserId(string userId);
}