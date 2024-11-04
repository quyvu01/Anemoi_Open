using System;
using System.Linq;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryManyFlow;

public interface IQueryListSpecialAction<TModel, TResponse> where TModel : class
{
    IQueryListSortedField<TModel, TResponse> WithSpecialAction(
        Func<IQueryable<TModel>, IQueryable<TModel>> specialAction);

    IQueryListSortedField<TModel, TResponse> WithSpecialAction(
        Func<IQueryable<TModel>, IQueryable<TResponse>> specialAction);
}