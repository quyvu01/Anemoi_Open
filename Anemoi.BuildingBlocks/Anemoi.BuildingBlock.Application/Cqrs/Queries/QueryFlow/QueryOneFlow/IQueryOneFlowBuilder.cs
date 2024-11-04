using System;
using System.Linq;
using System.Linq.Expressions;
using Anemoi.BuildingBlock.Application.Errors;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;

public interface IQueryOneFlowBuilder<TModel, out TResponse> where TModel : class where TResponse : class
{
    QuerySpecialActionType QuerySpecialActionType { get; }
    Expression<Func<TModel, bool>> Filter { get; }
    Func<IQueryable<TModel>, IQueryable<TModel>> SpecialAction { get; }
    Func<IQueryable<TModel>, IQueryable<TResponse>> SpecialActionToResponse { get; }
    ErrorDetail ErrorDetail { get; }
}