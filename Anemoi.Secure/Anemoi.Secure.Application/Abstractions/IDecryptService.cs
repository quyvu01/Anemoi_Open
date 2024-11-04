using System.Security.Cryptography;

namespace Anemoi.Secure.Application.Abstractions;

public interface IDecryptService
{
    byte[] Decrypt(byte[] encrypted, RSAEncryptionPadding rsaEncryptionPadding);
}