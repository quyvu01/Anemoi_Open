namespace Anemoi.BuildingBlock.Application.Configurations;

public sealed class MassTransitSetting
{
    public string Host { get; set; }
    public string VirtualHost { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool IsSslActive { get; set; }
    public string SslThumbprint { get; set; }

    public void Deconstruct(out string host, out string virtualHost, out string userName, out string password,
        out bool isSslActive, out string sslThumbprint)
        => (host, virtualHost, userName, password, isSslActive, sslThumbprint) =
            (Host, VirtualHost, UserName, Password, IsSslActive, SslThumbprint);
}