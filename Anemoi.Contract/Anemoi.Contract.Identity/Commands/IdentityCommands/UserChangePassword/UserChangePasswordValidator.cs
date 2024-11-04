using FluentValidation;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.UserChangePassword;

public sealed class UserChangePasswordValidator : AbstractValidator<UserChangePasswordCommand>
{
    public UserChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty();
    }
}