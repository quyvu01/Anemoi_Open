using FluentValidation;

namespace Anemoi.Contract.MasterData.Commands.ProvinceCommands.UpdateProvince;

public sealed class UpdateProvinceValidator : AbstractValidator<UpdateProvinceCommand>
{
    public UpdateProvinceValidator()
    {
        When(x => x.Name is { },
            () => RuleFor(x => x.Name).NotEmpty());
        RuleFor(x => x.Id)
            .Must(x => x is { } && x.Value != Guid.Empty)
            .WithMessage("ProvinceId cannot be empty!");
    }
}