using MassTransit;

namespace Anemoi.BuildingBlock.Application.Queries;

[ExcludeFromTopology]
public record GetManyQuery
{
    public int? PageSize { get; set; }
    public int? PageIndex { get; set; }

    public string SortedFieldName { get; set; }

    public SortedDirection? SortedDirection { get; set; }

    public int? GetSkip() => (PageIndex, PageSize) switch
    {
        ({ } pageIndex, { } pageSize) => (pageIndex - 1) * pageSize,
        _ => null
    };

    public int? GetTake() => PageSize;

    public void Deconstruct(out int? skip, out int? take, out string sortedFieldName,
        out SortedDirection? sortedDirection) => (skip, take, sortedFieldName, sortedDirection) = (
        (PageIndex, PageSize) switch
        {
            ({ } pageIndex, { } pageSize) => (pageIndex - 1) * pageSize,
            _ => null
        }, PageSize, SortedFieldName, SortedDirection);
}