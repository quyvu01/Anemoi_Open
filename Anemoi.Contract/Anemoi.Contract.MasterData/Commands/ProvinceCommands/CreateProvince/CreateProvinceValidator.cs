using FluentValidation;

namespace Anemoi.Contract.MasterData.Commands.ProvinceCommands.CreateProvince;

public sealed class CreateProvinceValidator : AbstractValidator<CreateProvinceCommand>
{
    public CreateProvinceValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}