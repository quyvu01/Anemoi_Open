using System;
using System.Linq;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;

public interface IUpdateManySpecialActionResult<TModel, TResult> where TModel : class
{
    IUpdateManyConditionResult<TModel, TResult> WithSpecialAction(
        Func<IQueryable<TModel>, IQueryable<TModel>> specialAction);
}

public interface IUpdateManySpecialActionVoid<TModel> where TModel : class
{
    IUpdateManyConditionVoid<TModel> WithSpecialAction(
        Func<IQueryable<TModel>, IQueryable<TModel>> specialAction);
}