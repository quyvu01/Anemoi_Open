using FluentValidation;

namespace Anemoi.Contract.Identity.Commands.UserCommands.UpdateUser;

public sealed class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        When(x => x.FirstName is { }, () => RuleFor(x => x.FirstName).NotEmpty());

        When(x => x.LastName is { }, () => RuleFor(x => x.LastName).NotEmpty());
    }
}