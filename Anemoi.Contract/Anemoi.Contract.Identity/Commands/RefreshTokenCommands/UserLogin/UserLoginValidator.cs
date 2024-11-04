using FluentValidation;

namespace Anemoi.Contract.Identity.Commands.RefreshTokenCommands.UserLogin;

public sealed class UserLoginValidator : AbstractValidator<UserLoginCommand>
{
    public UserLoginValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}