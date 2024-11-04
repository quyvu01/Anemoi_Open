using FluentValidation;

namespace Anemoi.Contract.MasterData.Queries.ProvinceQueries.GetProvince;

public sealed class GetProvinceValidator : AbstractValidator<GetProvinceQuery>
{
    public GetProvinceValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .Must(x => x is { } && x.Value != Guid.Empty)
            .WithMessage("ProvinceId cannot be empty!");
    }
}