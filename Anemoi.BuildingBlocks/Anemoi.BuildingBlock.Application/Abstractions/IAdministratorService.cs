namespace Anemoi.BuildingBlock.Application.Abstractions;

public interface IAdministratorGetter
{
    bool IsAdministrator { get; }
}

public interface IAdministratorSetter
{
    void SetAdministrator(bool isAdministrator);
}