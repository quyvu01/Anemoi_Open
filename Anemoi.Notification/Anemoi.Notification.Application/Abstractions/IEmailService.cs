using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using OneOf;

namespace Anemoi.Notification.Application.Abstractions;

public interface IEmailService
{
    Task SendEmailAsync(string mailTo, string subject, string body, List<string> attachments, List<string> cc = null,
        List<string> bcc = null, CancellationToken token = default);

    Task<OneOf<None, Exception>> AuthenticateSmtpAsync(string host, int port, string userName, string password,
        CancellationToken token = default);

    Task<OneOf<None, ErrorDetailResponse>> SendTemporaryEmailAsync(string host, int port, string userName,
        string password, string emailTo, string htmlBody, CancellationToken token = default);
}