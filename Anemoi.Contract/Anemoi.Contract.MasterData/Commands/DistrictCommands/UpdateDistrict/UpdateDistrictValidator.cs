using FluentValidation;

namespace Anemoi.Contract.MasterData.Commands.DistrictCommands.UpdateDistrict;

public sealed class UpdateDistrictValidator : AbstractValidator<UpdateDistrictCommand>
{
    public UpdateDistrictValidator()
    {
        When(x => x.Name is { }, () => RuleFor(x => x.Name).NotEmpty());
        When(x => x.ProvinceId is { }, () => RuleFor(x => x.ProvinceId)
            .Must(x => x is { } && x.Value != Guid.Empty)
            .WithMessage("ProvinceId cannot be empty!"));
        When(x => x.Id is { }, () => RuleFor(x => x.Id)
            .Must(x => x is { } && x.Value != Guid.Empty)
            .WithMessage("DistrictId cannot be empty!"));
        ;
    }
}