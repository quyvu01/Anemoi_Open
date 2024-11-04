namespace Anemoi.Notification.Application.ApplicationModels;

public sealed record SmtpConfiguration(string Name, string UserName, string Password, string Host, int Port);