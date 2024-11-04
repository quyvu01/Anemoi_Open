using System;
using System.Linq.Expressions;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.CountingFlow;

public class CountingFlow<TModel> :
    ICountingFilter<TModel>,
    ICountingFlowBuilder<TModel> where TModel : class
{
    public ICountingFlowBuilder<TModel> WithFilter(Expression<Func<TModel, bool>> filter)
    {
        Filter = filter;
        return this;
    }

    public Expression<Func<TModel, bool>> Filter { get; private set; }
}