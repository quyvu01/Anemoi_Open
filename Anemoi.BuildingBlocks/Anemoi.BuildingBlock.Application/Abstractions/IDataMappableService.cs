using System.Threading;
using System.Threading.Tasks;

namespace Anemoi.BuildingBlock.Application.Abstractions;

public interface IDataMappableService
{
    Task MapDataAsync(object value, CancellationToken token = default);
}