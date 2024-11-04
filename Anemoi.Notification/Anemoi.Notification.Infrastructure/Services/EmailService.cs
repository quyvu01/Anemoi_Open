using System.Net.Sockets;
using System.Web;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Serilog;
using Anemoi.Notification.Application.Abstractions;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using OneOf;
using Anemoi.Notification.Application.ApplicationModels;

namespace Anemoi.Notification.Infrastructure.Services;

public sealed class EmailService(
    ISmtpProvider smtpProvider,
    ILogger logger,
    EmailErrorMessages emailErrorMessages,
    IFileService fileService) : IEmailService
{
    private const int SslPort = 465;

    public async Task SendEmailAsync(string mailTo, string subject, string body, List<string> attachments,
        List<string> cc = null, List<string> bcc = null, CancellationToken token = default)
    {
        if (string.IsNullOrEmpty(mailTo))
            throw new ArgumentException(emailErrorMessages.EmailNil);
        if (!ValidationHelper.IsEmail(mailTo))
            throw new ArgumentException($"{nameof(mailTo)} is not valid!");
        if (bcc?.Any(b => !ValidationHelper.IsEmail(b)) ?? false)
            throw new ArgumentException($"{nameof(bcc)} is not valid!");
        if (cc?.Any(b => !ValidationHelper.IsEmail(b)) ?? false)
            throw new ArgumentException($"{nameof(cc)} is not valid!");

        var smtpConfiguration = await smtpProvider.SmtpConfiguration;
        if (smtpConfiguration is null)
        {
            logger.Information("[SmtpConfiguration] is not available!");
            return;
        }

        var emailFrom = smtpConfiguration.Name is { } name
            ? $"{name} <{smtpConfiguration.UserName}>"
            : smtpConfiguration.UserName;

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(emailFrom));
        email.To.Add(MailboxAddress.Parse(mailTo));
        if (bcc is { Count: > 0 }) email.Bcc.AddRange(bcc.Select(InternetAddress.Parse));
        if (cc is { Count: > 0 }) email.Cc.AddRange(cc.Select(InternetAddress.Parse));
        email.Subject = subject;
        var builder = new BodyBuilder { HtmlBody = body };
        if (attachments is { Count: > 0 })
        {
            var filesDataTasks = attachments.Select(async a =>
            {
                logger.Information("Attachment file: {@FilePath}", a);
                var result = await fileService
                    .GetFileAsync(a, token);
                var fileMetadata = await fileService
                    .GetFileMetadataAsync(a, "original-file-name", token);
                if (result.IsT1) return;
                var fileName = fileMetadata.Match(x => x, _ => a);
                var fileData = result.AsT0;
                await builder.Attachments.AddAsync(HttpUtility.UrlDecode(fileName), fileData.FileContent,
                    token);
            });
            await Task.WhenAll(filesDataTasks);
        }

        email.Body = builder.ToMessageBody();
        using var smtp = new SmtpClient();
        var useSsl = smtpConfiguration.Port == SslPort;
        await smtp.ConnectAsync(smtpConfiguration.Host, smtpConfiguration.Port, useSsl, token);
        await smtp.AuthenticateAsync(smtpConfiguration.UserName, smtpConfiguration.Password,
            token);
        await smtp.SendAsync(email, token);
        await smtp.DisconnectAsync(true, token);
    }

    public async Task<OneOf<None, Exception>> AuthenticateSmtpAsync(string host, int port, string userName,
        string password, CancellationToken token = default)
    {
        try
        {
            using var smtp = new SmtpClient();
            var useSsl = port == SslPort;
            await smtp.ConnectAsync(host, port, useSsl, token);
            await smtp.AuthenticateAsync(userName, password, token);
            await smtp.DisconnectAsync(true, token);
            return None.Value;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<OneOf<None, ErrorDetailResponse>> SendTemporaryEmailAsync(string host, int port, string userName,
        string password, string emailTo, string htmlBody, CancellationToken token = default)
    {
        try
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(userName));
            emailMessage.To.Add(MailboxAddress.Parse(emailTo));
            emailMessage.Subject = "SendTemporaryEmail!";
            var builder = new BodyBuilder { HtmlBody = htmlBody };
            emailMessage.Body = builder.ToMessageBody();
            emailMessage.From.Add(MailboxAddress.Parse(userName));
            using var smtp = new SmtpClient();
            var useSsl = port == SslPort;
            await smtp.ConnectAsync(host, port, useSsl, token);
            await smtp.AuthenticateAsync(userName, password, token);
            await smtp.SendAsync(emailMessage, token);
            await smtp.DisconnectAsync(true, token);
            return None.Value;
        }
        catch (AuthenticationException e)
        {
            return new ErrorDetailResponse { Code = "TER_07", Messages = new[] { e.Message } };
        }
        catch (SocketException e)
        {
            return new ErrorDetailResponse { Code = "TER_08", Messages = new[] { e.Message } };
        }
        catch (SmtpCommandException e)
        {
            return new ErrorDetailResponse { Code = "TER_09", Messages = new[] { e.Message } };
        }
        catch (Exception e)
        {
            return new ErrorDetailResponse { Code = "TER_10", Messages = new[] { e.Message } };
        }
    }
}