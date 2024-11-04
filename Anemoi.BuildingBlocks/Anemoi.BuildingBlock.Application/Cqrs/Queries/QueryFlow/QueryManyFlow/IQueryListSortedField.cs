using System;
using System.Linq.Expressions;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;

public interface IQueryListSortedField<TModel, out TResponse> where TModel : class
{
    IQueryListSortedDirection<TModel, TResponse> WithSortFieldWhenNotSet(Expression<Func<TModel, object>> expression);
}