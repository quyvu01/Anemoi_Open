namespace Anemoi.BuildingBlock.Application.Abstractions;

public interface ITokenGetter
{
    string Token { get; }
}

public interface ITokenSetter
{
    void SetToken(string token);
}