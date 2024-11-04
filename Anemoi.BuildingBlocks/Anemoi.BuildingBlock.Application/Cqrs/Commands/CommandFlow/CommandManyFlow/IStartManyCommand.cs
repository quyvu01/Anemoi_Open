using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;

public interface IStartManyCommandResult<TModel, TResult> where TModel : class
{
    ICreateManyConditionResult<TModel, TResult> CreateMany(Func<Task<List<TModel>>> modelsFunc);
    ICreateManyConditionResult<TModel, TResult> CreateMany(Func<List<TModel>> modelsFunc);
    ICreateManyConditionResult<TModel, TResult> CreateMany(List<TModel> models);
    IUpdateManySpecialActionResult<TModel, TResult> UpdateMany(Expression<Func<TModel, bool>> filter);
    IRemoveManySpecialActionResult<TModel, TResult> RemoveMany(Expression<Func<TModel, bool>> filter);
}
public interface IStartManyCommandVoid<TModel> where TModel : class
{
    ICreateManyConditionVoid<TModel> CreateMany(Func<Task<List<TModel>>> modelsFunc);
    ICreateManyConditionVoid<TModel> CreateMany(Func<List<TModel>> modelsFunc);
    ICreateManyConditionVoid<TModel> CreateMany(List<TModel> models);
    IUpdateManySpecialActionVoid<TModel> UpdateMany(Expression<Func<TModel, bool>> filter);
    IRemoveManySpecialActionVoid<TModel> RemoveMany(Expression<Func<TModel, bool>> filter);
}