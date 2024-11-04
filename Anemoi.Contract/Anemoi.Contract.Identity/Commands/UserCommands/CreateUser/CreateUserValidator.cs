using FluentValidation;

namespace Anemoi.Contract.Identity.Commands.UserCommands.CreateUser;

public sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Password).NotEmpty();
        When(x => x.FirstName is null, () => RuleFor(x => x.LastName).NotEmpty());
        When(x => x.LastName is null, () => RuleFor(x => x.FirstName).NotEmpty());
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}