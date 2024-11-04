using System;
using System.Linq;
using System.Linq.Expressions;
using Anemoi.BuildingBlock.Application.Queries;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;

public interface IQueryListFlowBuilder<TModel, out TResponse> where TModel : class
{
    QuerySpecialActionType QuerySpecialActionType { get; }
    Expression<Func<TModel, bool>> Filter { get; }
    Func<IQueryable<TModel>, IQueryable<TModel>> SpecialActionToModel { get; }
    Func<IQueryable<TModel>, IQueryable<TResponse>> SpecialActionToResponse { get; }
    Expression<Func<TModel, object>> SortFieldNameWhenRequestEmpty { get; }
    SortedDirection SortedDirectionWhenRequestEmpty { get; }
}