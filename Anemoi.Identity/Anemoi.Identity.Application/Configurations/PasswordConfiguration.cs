namespace Anemoi.Identity.Application.Configurations;

public sealed class PasswordConfiguration
{
    public bool IncludeLowercase { get; set; }
    public bool IncludeUppercase { get; set; }
    public bool IncludeNumeric { get; set; }
    public bool IncludeSpecial { get; set; }
    public int PasswordLength { get; set; }

    public void Deconstruct(out bool includeLowercase, out bool includeUppercase, out bool includeNumeric,
        out bool includeSpecial, out int passwordLength)
        => (includeLowercase, includeUppercase, includeNumeric, includeSpecial, passwordLength) = (IncludeLowercase,
            IncludeUppercase, IncludeNumeric, IncludeSpecial, PasswordLength);
}