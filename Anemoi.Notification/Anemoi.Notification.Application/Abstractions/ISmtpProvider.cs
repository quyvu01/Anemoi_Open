using Anemoi.Notification.Application.ApplicationModels;

namespace Anemoi.Notification.Application.Abstractions;

public interface ISmtpProvider
{
    Task<SmtpConfiguration> SmtpConfiguration { get; }
}