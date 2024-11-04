using FluentValidation;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.CheckEmailResetTokenCommand;

public sealed class CheckEmailResetTokenValidator : AbstractValidator<CheckEmailResetTokenCommand>
{
    public CheckEmailResetTokenValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty();
        RuleFor(x => x.Code)
            .NotEmpty();
    }
}