using System;
using System.Linq.Expressions;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.CountingFlow;

public interface ICountingFlowBuilder<TModel> where TModel : class
{
    Expression<Func<TModel, bool>> Filter { get; }
}