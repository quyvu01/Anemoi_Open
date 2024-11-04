namespace Anemoi.Secure.Application.Configurations;

public class OtpSetting
{
    public int Step { get; set; } // seconds
    public string PrivateKey { get; set; }
}