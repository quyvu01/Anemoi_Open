using System;

namespace Anemoi.BuildingBlock.Application.Configurations;

public sealed class JwtSetting
{
    public string PrivateKeyPath { get; set; }
    public string PublicKeyPath { get; set; }
    public TimeSpan TokenLifetime { get; set; }
    public TimeSpan RefreshTokenLifetime { get; set; }
}