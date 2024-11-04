using System.Security.Cryptography;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Configurations;
using Anemoi.Secure.Application.Abstractions;
using Anemoi.Secure.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Secure.Infrastructure.Installers;

public sealed class EncryptInstaller : IInstaller
{
    private const string BeginPublicKey = "-----BEGIN RSA PUBLIC KEY-----";
    private const string EndPublicKey = "-----END RSA PUBLIC KEY-----";

    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        var certifications = configuration.GetSection(nameof(Certifications)).Get<Certifications>()!;
        var publicKeyRaw = File.ReadAllText(certifications.EncryptPath);
        var publicKey = new[] { BeginPublicKey, EndPublicKey }
            .Aggregate(publicKeyRaw, (acc, next) => acc.Replace(next, string.Empty));
        var publicKeyBytes = Convert.FromBase64String(publicKey);
        var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(new ReadOnlySpan<byte>(publicKeyBytes), out _);
        services.AddSingleton<IEncryptService>(_ => new EncryptService(rsa));
    }
}