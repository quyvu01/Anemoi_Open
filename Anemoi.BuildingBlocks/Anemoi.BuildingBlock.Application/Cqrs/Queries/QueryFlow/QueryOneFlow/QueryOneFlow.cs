using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Anemoi.BuildingBlock.Application.Errors;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;

public class QueryOneFlow<TModel, TResponse> :
    IQueryOneFilter<TModel, TResponse>,
    IQueryOneSpecialAction<TModel, TResponse>,
    IQueryOneErrorDetail<TModel, TResponse>,
    IQueryOneFlowBuilder<TModel, TResponse> where TModel : class where TResponse : class
{
    public QuerySpecialActionType QuerySpecialActionType { get; private set; }
    public Expression<Func<TModel, bool>> Filter { get; private set; }
    public Func<IQueryable<TModel>, IQueryable<TModel>> SpecialAction { get; private set; }
    public Func<IQueryable<TModel>, IQueryable<TResponse>> SpecialActionToResponse { get; private set; }
    public ErrorDetail ErrorDetail { get; private set; }

    public IQueryOneSpecialAction<TModel, TResponse> WithFilter(Expression<Func<TModel, bool>> filter)
    {
        Filter = filter;
        return this;
    }

    public IQueryOneErrorDetail<TModel, TResponse> WithSpecialAction(Func<IQueryable<TModel>, IQueryable<TModel>> specialAction)
    {
        QuerySpecialActionType = QuerySpecialActionType.ToModel;
        SpecialAction = specialAction;
        return this;
    }

    public IQueryOneErrorDetail<TModel, TResponse> WithSpecialAction(Func<IQueryable<TModel>, IQueryable<TResponse>> specialAction)
    {
        QuerySpecialActionType = QuerySpecialActionType.ToTarget;
        SpecialActionToResponse = specialAction;
        return this;
    }

    public IQueryOneFlowBuilder<TModel, TResponse> WithErrorIfNull([NotNull] ErrorDetail errorDetail)
    {
        ErrorDetail = errorDetail;
        return this;
    }
}