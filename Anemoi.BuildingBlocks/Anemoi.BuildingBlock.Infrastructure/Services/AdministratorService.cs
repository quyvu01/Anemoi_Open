using Anemoi.BuildingBlock.Application.Abstractions;

namespace Anemoi.BuildingBlock.Infrastructure.Services;

public sealed class AdministratorService : IAdministratorSetter, IAdministratorGetter
{
    public void SetAdministrator(bool isAdministrator) => IsAdministrator = isAdministrator;

    public bool IsAdministrator { get; private set; }
}