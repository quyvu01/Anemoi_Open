using System.Text;
using OtpNet;
using Anemoi.Secure.Application.Abstractions;
using Anemoi.Secure.Application.Configurations;

namespace Anemoi.Secure.Infrastructure.Services;

public class OtpService(OtpSetting otpSetting) : IOtpService
{
    public Task<string> GenerateOtpAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var totp = new Totp(Encoding.UTF8.GetBytes($"{phoneNumber}-{otpSetting.PrivateKey}"), step: otpSetting.Step,
            OtpHashMode.Sha256);
        var code = totp.ComputeTotp();
        return Task.FromResult(code);
    }

    public Task<bool> ValidateOtpAsync(string phoneNumber, string otp, CancellationToken cancellationToken = default)
    {
        var totp = new Totp(Encoding.UTF8.GetBytes($"{phoneNumber}-{otpSetting.PrivateKey}"), step: otpSetting.Step,
            OtpHashMode.Sha256);
        var isValid = totp.VerifyTotp(otp, out _);
        return Task.FromResult(isValid);
    }
}