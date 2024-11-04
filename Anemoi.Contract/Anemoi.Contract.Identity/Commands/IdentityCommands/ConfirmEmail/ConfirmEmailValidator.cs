using FluentValidation;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.ConfirmEmail;

public sealed class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();
        RuleFor(x => x.Token)
            .NotEmpty();
    }
}