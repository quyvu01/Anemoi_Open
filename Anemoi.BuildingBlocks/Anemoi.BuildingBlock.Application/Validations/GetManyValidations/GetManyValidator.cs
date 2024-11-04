using Anemoi.BuildingBlock.Application.Queries;
using FluentValidation;

namespace Anemoi.BuildingBlock.Application.Validations.GetManyValidations;

public class GetManyValidator<T> : AbstractValidator<T> where T : GetManyQuery
{
    protected GetManyValidator()
    {
        When(x => x.PageIndex is { }, () => RuleFor(x => x.PageIndex).GreaterThan(0));
        When(x => x.PageSize is { }, () => RuleFor(x => x.PageSize).GreaterThan(0));
    }
}