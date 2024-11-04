namespace Anemoi.BuildingBlock.Application.Responses;

public class PaginationResponseGeneral
{
    public long TotalRecord { get; set; }
    public int CurrentPageIndex { get; set; }
    public int TotalPage { get; set; }
}