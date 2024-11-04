namespace Anemoi.Secure.Application.Abstractions;

public interface IOtpService
{
    Task<string> GenerateOtpAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<bool> ValidateOtpAsync(string phoneNumber, string otp, CancellationToken cancellationToken = default);
}