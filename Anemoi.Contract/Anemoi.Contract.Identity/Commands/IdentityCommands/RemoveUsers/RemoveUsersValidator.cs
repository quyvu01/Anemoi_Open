using FluentValidation;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.RemoveUsers;

public sealed class RemoveUsersValidator : AbstractValidator<RemoveUsersCommand>
{
    public RemoveUsersValidator()
    {
        RuleFor(x => x.UserIds)
            .NotEmpty()
            .Must(x => x.All(id => id is { } && id.Value != Guid.Empty))
            .WithMessage("Some Ids are empty!")
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("Some Ids are duplicated!");
    }
}