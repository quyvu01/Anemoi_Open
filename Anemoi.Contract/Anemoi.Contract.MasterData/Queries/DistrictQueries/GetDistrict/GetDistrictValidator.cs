using FluentValidation;

namespace Anemoi.Contract.MasterData.Queries.DistrictQueries.GetDistrict;

public sealed class GetDistrictValidator : AbstractValidator<GetDistrictQuery>
{
    public GetDistrictValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .Must(x => x is { } && x.Value != Guid.Empty)
            .WithMessage("DistrictId cannot be empty!");
    }
}