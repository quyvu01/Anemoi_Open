using FluentValidation;

namespace Anemoi.Contract.Identity.Commands.RoleGroupCommands.UpdateRoleGroup;

public sealed class UpdateRoleGroupValidator : AbstractValidator<UpdateRoleGroupCommand>
{
    public UpdateRoleGroupValidator()
    {
        When(x => x.Name is { }, () => RuleFor(x => x.Name).NotEmpty());

        When(x => x.IdentityRoleIds is { },
            () => RuleFor(x => x.IdentityRoleIds).Must(ids => ids is { } && ids.Count == ids.Distinct().Count())
                .WithMessage("Role Ids must not be duplicated!"));
    }
}