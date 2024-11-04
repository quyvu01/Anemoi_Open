using FluentValidation;

namespace Anemoi.Contract.Workspace.Queries.OrganizationQueries.GetOrganization;

public sealed class GetOrganizationValidator : AbstractValidator<GetOrganizationQuery>
{
    public GetOrganizationValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Organization Id can not be empty!");
    }
}