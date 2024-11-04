using FluentValidation;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.ChangePasswordWithToken;

public sealed class ChangePasswordWithTokenValidator : AbstractValidator<ChangePasswordWithTokenCommand>
{
    public ChangePasswordWithTokenValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();
        RuleFor(x => x.Email)
            .EmailAddress();
        RuleFor(x => x.NewPassword)
            .Must(x => !string.IsNullOrWhiteSpace(x));
    }
}