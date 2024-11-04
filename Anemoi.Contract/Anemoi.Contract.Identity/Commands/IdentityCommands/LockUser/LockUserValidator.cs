using FluentValidation;

namespace Anemoi.Contract.Identity.Commands.IdentityCommands.LockUser;

public sealed class LockUserValidator : AbstractValidator<LockUserCommand>
{
    public LockUserValidator()
    {
        RuleFor(x => x.LockUntil)
            .Must((lockCommand, time) => !lockCommand.EnableLock || time is { } && time > DateTime.UtcNow)
            .WithMessage("Lock until must be larger than current time!");
    }
}