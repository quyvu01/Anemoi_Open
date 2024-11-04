using System.Security.Cryptography;

namespace Anemoi.Secure.Application.Abstractions;

public interface IEncryptService
{
    byte[] Encrypt(byte[] encrypted, RSAEncryptionPadding rsaEncryptionPadding);
}