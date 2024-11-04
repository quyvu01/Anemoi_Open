using Anemoi.BuildingBlock.Application.Abstractions;

namespace Anemoi.BuildingBlock.Infrastructure.Services;

public sealed class TokenService : ITokenSetter, ITokenGetter
{
    public void SetToken(string token) => Token = token;

    public string Token { get; private set; }
}