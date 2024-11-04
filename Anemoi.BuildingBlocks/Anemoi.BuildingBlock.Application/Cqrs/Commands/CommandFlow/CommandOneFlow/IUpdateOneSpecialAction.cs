using System;
using System.Linq;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;

public interface IUpdateOneSpecialActionResult<TModel, TResult> where TModel : class
{
    IUpdateOneConditionResult<TModel, TResult>
        WithSpecialAction(Func<IQueryable<TModel>, IQueryable<TModel>> specialAction);
}

public interface IUpdateOneSpecialActionVoid<TModel> where TModel : class
{
    IUpdateOneConditionVoid<TModel> WithSpecialAction(Func<IQueryable<TModel>, IQueryable<TModel>> specialAction);
}