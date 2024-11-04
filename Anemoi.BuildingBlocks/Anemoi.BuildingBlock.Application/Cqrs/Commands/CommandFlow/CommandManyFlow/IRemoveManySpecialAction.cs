using System;
using System.Linq;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;

public interface IRemoveManySpecialActionResult<TModel, TResult> where TModel : class
{
    IRemoveManyConditionResult<TModel, TResult> WithSpecialAction(
        Func<IQueryable<TModel>, IQueryable<TModel>> specialAction);
}

public interface IRemoveManySpecialActionVoid<TModel> where TModel : class
{
    IRemoveManyConditionVoid<TModel>
        WithSpecialAction(Func<IQueryable<TModel>, IQueryable<TModel>> specialAction);
}