using System.Security.Cryptography;
using Anemoi.Secure.Application.Abstractions;

namespace Anemoi.Secure.Infrastructure.Services;

public sealed class DecryptService(RSA rsa) : IDecryptService
{
    public byte[] Decrypt(byte[] encrypted, RSAEncryptionPadding rsaEncryptionPadding) =>
        rsa.Decrypt(encrypted, rsaEncryptionPadding);
}