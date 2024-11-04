using FluentValidation;

namespace Anemoi.Contract.Identity.Commands.RoleGroupCommands.CreateRoleGroup;

public sealed class CreateRoleGroupValidator : AbstractValidator<CreateRoleGroupCommand>
{
    public CreateRoleGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.IdentityRoleIds)
            .NotEmpty()
            .Must(ids => ids.Count == ids.Distinct().Count())
            .WithMessage("Role Ids must not be duplicated!");
    }
}