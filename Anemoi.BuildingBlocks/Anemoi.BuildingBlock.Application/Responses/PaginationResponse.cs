using System.Collections.Generic;

namespace Anemoi.BuildingBlock.Application.Responses;

public sealed class PaginationResponse<T> : PaginationResponseGeneral where T : class
{
    public List<T> Items { get; set; }

    public PaginationResponse()
    {
    }

    public PaginationResponse(List<T> items, long totalRecord)
        => (Items, TotalRecord) = (items, totalRecord);
}