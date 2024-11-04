using Anemoi.BuildingBlock.Application.ApplicationModels;

namespace Anemoi.BuildingBlock.Application.Abstractions;

public interface IHeaderContextProvider
{
    HeaderContext CreateContext();
    HeaderContext GetContext();
}