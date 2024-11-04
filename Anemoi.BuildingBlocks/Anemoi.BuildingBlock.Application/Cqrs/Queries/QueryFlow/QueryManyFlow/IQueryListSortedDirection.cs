using Anemoi.BuildingBlock.Application.Queries;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;

public interface IQueryListSortedDirection<TModel, out TResponse> where TModel : class
{
    IQueryListFlowBuilder<TModel, TResponse> WithSortedDirectionWhenNotSet(SortedDirection direction);
}