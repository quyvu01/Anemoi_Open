using FluentValidation;

namespace Anemoi.Contract.MasterData.Commands.DistrictCommands.CreateDistrict;

public sealed class CreateDistrictValidator : AbstractValidator<CreateDistrictCommand>
{
    public CreateDistrictValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.ProvinceId)
            .Must(x => x is { } && x.Value != Guid.Empty)
            .WithMessage("ProvinceId cannot be empty!");
    }
}