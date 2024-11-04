using System;
using System.Linq.Expressions;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;

public interface IQueryOneFilter<TModel, TResponse> where TModel : class where TResponse : class
{
    IQueryOneSpecialAction<TModel, TResponse> WithFilter(Expression<Func<TModel, bool>> filter);
}