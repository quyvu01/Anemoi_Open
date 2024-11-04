using System.Collections.Generic;

namespace Anemoi.BuildingBlock.Domain.Models;

public sealed class Pagination<T>
{
    public IEnumerable<T> Items { get; set; }
    public long TotalRecord { get; set; }
}