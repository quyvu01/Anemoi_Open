using System.Collections.Generic;

namespace Anemoi.BuildingBlock.Application.Responses;

public sealed class CollectionResponse<T> where T : class
{
    public List<T> Items { get; set; }
    public long TotalRecord { get; set; }

    public CollectionResponse()
    {
    }

    public CollectionResponse(List<T> items, long totalRecord) => (Items, TotalRecord) = (items, totalRecord);
    public CollectionResponse(List<T> items) => (Items, TotalRecord) = (items, items.Count);
}