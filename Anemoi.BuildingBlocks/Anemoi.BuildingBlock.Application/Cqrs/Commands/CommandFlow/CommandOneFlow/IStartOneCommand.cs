using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;

public interface IStartOneCommandResult<TModel, TResult> where TModel : class
{
    ICreateOneConditionResult<TModel, TResult> CreateOne(Func<Task<TModel>> modelFunc);
    ICreateOneConditionResult<TModel, TResult> CreateOne(Func<TModel> modelFunc);
    ICreateOneConditionResult<TModel, TResult> CreateOne(TModel model);
    IUpdateOneSpecialActionResult<TModel, TResult> UpdateOne(Expression<Func<TModel, bool>> filter);
    IRemoveOneSpecialActionResult<TModel, TResult> RemoveOne(Expression<Func<TModel, bool>> filter);
}

public interface IStartOneCommandVoid<TModel> where TModel : class
{
    ICreateOneConditionVoid<TModel> CreateOne(Func<Task<TModel>> modelFunc);
    ICreateOneConditionVoid<TModel> CreateOne(Func<TModel> modelFunc);
    ICreateOneConditionVoid<TModel> CreateOne(TModel model);
    IUpdateOneSpecialActionVoid<TModel> UpdateOne(Expression<Func<TModel, bool>> filter);
    IRemoveOneSpecialActionVoid<TModel> RemoveOne(Expression<Func<TModel, bool>> filter);
}