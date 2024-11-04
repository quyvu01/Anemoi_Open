using System;
using System.Linq;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;

public interface IQueryOneSpecialAction<TModel, TResponse> where TModel : class where TResponse : class
{
    IQueryOneErrorDetail<TModel, TResponse> WithSpecialAction(Func<IQueryable<TModel>, IQueryable<TModel>> specialAction);
    IQueryOneErrorDetail<TModel, TResponse> WithSpecialAction(Func<IQueryable<TModel>, IQueryable<TResponse>> specialAction);
}