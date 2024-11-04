using System.Security.Cryptography;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.Contract.Secure.Errors;
using Anemoi.Contract.Secure.Queries.GetDecryptData;
using Anemoi.Contract.Secure.Queries.GetEncryptData;
using Anemoi.Contract.Secure.Responses;
using Anemoi.Secure.Application.Abstractions;
using MassTransit;
using Serilog;

namespace Anemoi.Secure.Application.EventDriven.QueryHandlers;

public sealed class DecryptHandlers(
    IDecryptService decryptService,
    ILogger logger,
    IEncryptService encryptService)
    :
        IConsumer<GetDecryptDataQuery>,
        IConsumer<GetEncryptDataQuery>
{
    public Task Consume(ConsumeContext<GetDecryptDataQuery> context)
    {
        var message = context.Message;
        try
        {
            var decryptData = decryptService.Decrypt(message.EncryptBytes, RSAEncryptionPadding.OaepSHA256);
            return context.RespondAsync<CryptographyResponse>(new { DecryptData = decryptData });
        }
        catch (Exception e)
        {
            logger.Information("Error while decrypting data: {@Error}", e.Message);
            return context.RespondAsync(SecureErrorDetail.CryptographyError.DecryptFailed().ToErrorDetailResponse());
        }
    }

    public Task Consume(ConsumeContext<GetEncryptDataQuery> context)
    {
        var message = context.Message;
        try
        {
            var decryptData = encryptService.Encrypt(message.RawBytesData, RSAEncryptionPadding.OaepSHA256);
            return context.RespondAsync<CryptographyResponse>(new { DecryptData = decryptData });
        }
        catch (Exception e)
        {
            logger.Information("Error while encrypting data: {@Error}", e.Message);
            return context.RespondAsync(SecureErrorDetail.CryptographyError.EncryptFailed().ToErrorDetailResponse());
        }
    }
}