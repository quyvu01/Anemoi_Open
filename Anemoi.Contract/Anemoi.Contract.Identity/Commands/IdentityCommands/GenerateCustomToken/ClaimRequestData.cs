namespace Anemoi.Contract.Identity.Commands.IdentityCommands.GenerateCustomToken;

public sealed class ClaimRequestData
{
    public string ClaimType { get; set; }
    public string ClaimValue { get; set; }
}