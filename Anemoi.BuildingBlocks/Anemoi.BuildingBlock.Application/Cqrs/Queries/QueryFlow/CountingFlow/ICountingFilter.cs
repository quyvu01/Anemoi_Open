using System;
using System.Linq.Expressions;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.CountingFlow;

public interface ICountingFilter<TModel> where TModel : class
{
    ICountingFlowBuilder<TModel> WithFilter(Expression<Func<TModel, bool>> filter);
}