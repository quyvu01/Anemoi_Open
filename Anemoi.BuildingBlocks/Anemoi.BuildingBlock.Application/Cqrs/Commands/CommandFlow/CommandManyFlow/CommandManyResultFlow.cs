using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Results;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;

public class CommandManyResultFlow<TModel, TResult> :
    IStartManyCommandResult<TModel, TResult>,
    ICreateManyConditionResult<TModel, TResult>,
    IUpdateManySpecialActionResult<TModel, TResult>,
    IRemoveManySpecialActionResult<TModel, TResult>,
    IUpdateManyConditionResult<TModel, TResult>,
    IRemoveManyConditionResult<TModel, TResult>,
    IUpdateManyModifyResult<TModel, TResult>,
    ICommandManyErrorDetailResult<TModel, TResult>,
    ISaveChangesManyErrorDetailResult<TModel, TResult>,
    ISaveChangesManySucceedResult<TModel, TResult>,
    ICommandManyFlowBuilderResult<TModel, TResult>
    where TModel : class
{
    public ICreateManyConditionResult<TModel, TResult> CreateMany(Func<Task<List<TModel>>> modelsFunc)
    {
        CommandTypeMany = CommandTypeMany.Create;
        ModelsCreateFunc = modelsFunc;
        return this;
    }

    public ICreateManyConditionResult<TModel, TResult> CreateMany(Func<List<TModel>> modelsFunc)
    {
        CommandTypeMany = CommandTypeMany.Create;
        ModelsCreateFunc = () => Task.FromResult(modelsFunc.Invoke());
        return this;
    }

    public ICreateManyConditionResult<TModel, TResult> CreateMany(List<TModel> models)
    {
        CommandTypeMany = CommandTypeMany.Create;
        ModelsCreateFunc = () => Task.FromResult(models);
        return this;
    }

    public ISaveChangesManyErrorDetailResult<TModel, TResult> WithCondition(
        Func<List<TModel>, OneOf<None, ErrorDetail>> condition)
    {
        CommandManyCondition = models => Task.FromResult(condition.Invoke(models));
        return this;
    }

    public ISaveChangesManyErrorDetailResult<TModel, TResult> WithCondition(
        Func<List<TModel>, Task<OneOf<None, ErrorDetail>>> conditionAsync)
    {
        CommandManyCondition = conditionAsync;
        return this;
    }

    IUpdateManyModifyResult<TModel, TResult> IUpdateManyConditionResult<TModel, TResult>.WithCondition(
        Func<List<TModel>, Task<OneOf<None, ErrorDetail>>> conditionAsync)
    {
        CommandManyCondition = conditionAsync;
        return this;
    }

    ISaveChangesManyErrorDetailResult<TModel, TResult> IRemoveManyConditionResult<TModel, TResult>.
        WithCondition(
            Func<List<TModel>, OneOf<None, ErrorDetail>> condition)
    {
        CommandManyCondition = models => Task.FromResult(condition.Invoke(models));
        return this;
    }

    ISaveChangesManyErrorDetailResult<TModel, TResult> IRemoveManyConditionResult<TModel, TResult>.
        WithCondition(
            Func<List<TModel>, Task<OneOf<None, ErrorDetail>>> conditionAsync)
    {
        CommandManyCondition = conditionAsync;
        return this;
    }

    IUpdateManyModifyResult<TModel, TResult> IUpdateManyConditionResult<TModel, TResult>.WithCondition(
        Func<List<TModel>, OneOf<None, ErrorDetail>> condition)
    {
        CommandManyCondition = models => Task.FromResult(condition.Invoke(models));
        return this;
    }

    public IUpdateManySpecialActionResult<TModel, TResult> UpdateMany(Expression<Func<TModel, bool>> filter)
    {
        CommandTypeMany = CommandTypeMany.Update;
        CommandFilter = filter;
        return this;
    }

    IUpdateManyConditionResult<TModel, TResult> IUpdateManySpecialActionResult<TModel, TResult>.
        WithSpecialAction(
            Func<IQueryable<TModel>, IQueryable<TModel>> specialAction)
    {
        CommandSpecialAction = specialAction;
        return this;
    }

    public ISaveChangesManyErrorDetailResult<TModel, TResult> WithModify(Func<List<TModel>, Task> updateFuncAsync)
    {
        UpdateManyFunc = updateFuncAsync;
        return this;
    }

    public ISaveChangesManyErrorDetailResult<TModel, TResult> WithModify(Action<List<TModel>> updateFunc)
    {
        UpdateManyFunc = models =>
        {
            updateFunc.Invoke(models);
            return Task.CompletedTask;
        };
        return this;
    }


    IRemoveManyConditionResult<TModel, TResult> IRemoveManySpecialActionResult<TModel, TResult>.
        WithSpecialAction(
            Func<IQueryable<TModel>, IQueryable<TModel>> specialAction)
    {
        CommandSpecialAction = specialAction;
        return this;
    }

    public IRemoveManySpecialActionResult<TModel, TResult> RemoveMany(Expression<Func<TModel, bool>> filter)
    {
        CommandTypeMany = CommandTypeMany.Remove;
        CommandFilter = filter;
        return this;
    }

    public ISaveChangesManySucceedResult<TModel, TResult> WithErrorIfSaveChange(ErrorDetail errorDetail)
    {
        SaveChangesErrorDetail = errorDetail;
        return this;
    }

    ISaveChangesManyErrorDetailResult<TModel, TResult> ICommandManyErrorDetailResult<TModel, TResult>.
        WithErrorIfNull(
            ErrorDetail errorDetail)
    {
        NullErrorDetail = errorDetail;
        return this;
    }

    public ICommandManyFlowBuilderResult<TModel, TResult> WithResultIfSucceed(
        Func<List<TModel>, TResult> resultFunc)
    {
        ResultFunc = resultFunc;
        return this;
    }

    public CommandTypeMany CommandTypeMany { get; private set; } = CommandTypeMany.Unknown;
    public Func<Task<List<TModel>>> ModelsCreateFunc { get; private set; }
    public Func<List<TModel>, Task<OneOf<None, ErrorDetail>>> CommandManyCondition { get; private set; }
    public Expression<Func<TModel, bool>> CommandFilter { get; private set; }
    public Func<IQueryable<TModel>, IQueryable<TModel>> CommandSpecialAction { get; private set; }
    public Func<List<TModel>, Task> UpdateManyFunc { get; private set; }
    public ErrorDetail NullErrorDetail { get; private set; }
    public ErrorDetail SaveChangesErrorDetail { get; private set; }
    public Func<List<TModel>, TResult> ResultFunc { get; private set; }
}