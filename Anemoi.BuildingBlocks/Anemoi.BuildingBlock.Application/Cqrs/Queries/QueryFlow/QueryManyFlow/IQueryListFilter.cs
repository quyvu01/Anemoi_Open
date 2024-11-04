using System;
using System.Linq.Expressions;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;

public interface IQueryListFilter<TModel, TResponse> where TModel : class
{
    IQueryListSpecialAction<TModel, TResponse> WithFilter(Expression<Func<TModel, bool>> filter);
}