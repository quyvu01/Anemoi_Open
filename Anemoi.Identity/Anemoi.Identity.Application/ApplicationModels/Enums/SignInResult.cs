namespace Anemoi.Identity.Application.ApplicationModels.Enums;

public enum SignInResult
{
    IsLockedOut,
    IsNotAllowed,
    RequiresTwoFactor,
    Succeeded
}