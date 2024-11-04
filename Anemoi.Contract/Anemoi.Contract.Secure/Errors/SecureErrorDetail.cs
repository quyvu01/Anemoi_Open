using Anemoi.BuildingBlock.Application.Errors;

namespace Anemoi.Contract.Secure.Errors;

public static class SecureErrorDetail
{
    public static class CryptographyError
    {
        public static ErrorDetail EncryptFailed() => new()
        {
            Messages = new[] { "Error while encrypting encrypted data!" }, Code = "SCE_01"
        };

        public static ErrorDetail DecryptFailed() => new()
        {
            Messages = new[] { "Error while decrypting encrypted data!" }, Code = "SCE_02"
        };
    }

    public static class OtpError
    {
        public static ErrorDetail OtpNotValid() => new()
        {
            Messages = new[] { "Otp is not valid!" }, Code = "OTP_01"
        };
    }
}