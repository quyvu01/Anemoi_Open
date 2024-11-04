using FluentValidation;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.ChangePasswordWithoutOldPassword;

public sealed class ChangePasswordWithoutOldPasswordValidator : AbstractValidator<ChangePasswordWithoutOldPasswordCommand>
{
    public ChangePasswordWithoutOldPasswordValidator()
    {
        RuleFor(x => x.NewPassword).NotEmpty();
    }
}