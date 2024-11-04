using System.Security.Cryptography;
using Anemoi.Secure.Application.Abstractions;

namespace Anemoi.Secure.Infrastructure.Services;

public sealed class EncryptService(RSA rsa) : IEncryptService
{
    public byte[] Encrypt(byte[] encrypted, RSAEncryptionPadding rsaEncryptionPadding) =>
        rsa.Encrypt(encrypted, rsaEncryptionPadding);
}