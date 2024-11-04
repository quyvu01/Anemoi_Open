using System.Security.Cryptography;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Configurations;
using Anemoi.Secure.Application.Abstractions;
using Anemoi.Secure.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Secure.Infrastructure.Installers;

public class DecryptInstaller : IInstaller
{
    private const string BeginPrivateKey = "-----BEGIN RSA PRIVATE KEY-----";
    private const string EndPrivateKey = "-----END RSA PRIVATE KEY-----";

    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        var certifications = configuration.GetSection(nameof(Certifications)).Get<Certifications>()!;
        var privateKeyRaw = File.ReadAllText(certifications.DecryptPath);
        var privateKey = new[] { BeginPrivateKey, EndPrivateKey }
            .Aggregate(privateKeyRaw, (acc, next) => acc.Replace(next, string.Empty));
        var privateKeyBytes = Convert.FromBase64String(privateKey);
        var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(new ReadOnlySpan<byte>(privateKeyBytes), out _);
        services.AddSingleton<IDecryptService>(_ => new DecryptService(rsa));
    }
}