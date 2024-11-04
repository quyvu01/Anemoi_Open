using FluentValidation;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.UserForgetPassword;

public sealed class UserForgetPasswordValidator : AbstractValidator<UserForgetPasswordCommand>
{
    public UserForgetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty();
    }
}