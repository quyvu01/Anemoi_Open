using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Helpers;

public static class JwtSecurity
{
    private const string BeginPrivateKey = "-----BEGIN RSA PRIVATE KEY-----";
    private const string EndPrivateKey = "-----END RSA PRIVATE KEY-----";
    private const string BeginPublicKey = "-----BEGIN RSA PUBLIC KEY-----";
    private const string EndPublicKey = "-----END RSA PUBLIC KEY-----";

    public static SigningCredentials GetPrivateSigningCredential(OneOf<SecurityKey, string> secureOrPrivateKeyPath)
    {
        var rsaSecurityKey = secureOrPrivateKeyPath.Match(s => s, GetPrivateSecurityKey);
        return new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);
    }

    public static SecurityKey GetPrivateSecurityKey(string privateKeyPath)
    {
        var privateKeyRaw = File.ReadAllText(privateKeyPath);
        var privateKey = new[] {BeginPrivateKey, EndPrivateKey}
            .Aggregate(privateKeyRaw, (acc, next) => acc.Replace(next, string.Empty));
        var privateKeyBytes = Convert.FromBase64String(privateKey);
        var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(new ReadOnlySpan<byte>(privateKeyBytes), out _);
        return new RsaSecurityKey(rsa);
    }

    public static SecurityKey GetPublicSigningCredential(string publicKeyPath)
    {
        var publicKeyRaw = File.ReadAllText(publicKeyPath);
        var publicKey = new[] {BeginPublicKey, EndPublicKey}
            .Aggregate(publicKeyRaw, (acc, next) => acc.Replace(next, string.Empty));
        var publicKeyBytes = Convert.FromBase64String(publicKey);
        var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(new ReadOnlySpan<byte>(publicKeyBytes), out _);
        return new RsaSecurityKey(rsa);
    }
}