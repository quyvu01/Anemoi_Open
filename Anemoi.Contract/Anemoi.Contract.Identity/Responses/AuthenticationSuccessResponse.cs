namespace Anemoi.Contract.Identity.Responses;

public sealed class AuthenticationSuccessResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiredIn { get; set; }
}