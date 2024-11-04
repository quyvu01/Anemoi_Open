using FluentValidation;

namespace Anemoi.Contract.Identity.Commands.RefreshTokenCommands.UserRefreshToken;

public sealed class UserRefreshTokenValidator : AbstractValidator<UserRefreshTokenCommand>
{
    public UserRefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotNull()
            .Must(rt => rt is { } && rt.Value != Guid.Empty)
            .WithMessage("Refresh token must not be empty!!");
    }
}