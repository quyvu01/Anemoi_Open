using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Results;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;

public class CommandOneResultFlow<TModel, TResult> :
    IStartOneCommandResult<TModel, TResult>,
    ICreateOneConditionResult<TModel, TResult>,
    IUpdateOneSpecialActionResult<TModel, TResult>,
    IRemoveOneSpecialActionResult<TModel, TResult>,
    IUpdateOneConditionResult<TModel, TResult>,
    IRemoveOneConditionResult<TModel, TResult>,
    IUpdateOneModifyResult<TModel, TResult>,
    ICommandOneErrorDetailResult<TModel, TResult>,
    ISaveChangesOneErrorDetailResult<TModel, TResult>,
    ISaveChangesOneSucceed<TModel, TResult>,
    ICommandOneFlowBuilderResult<TModel, TResult>
    where TModel : class
{
    public CommandTypeOne CommandTypeOne { get; private set; } = CommandTypeOne.Unknown;
    public Func<Task<TModel>> ModelCreateFunc { get; private set; }
    public Func<TModel, Task<OneOf<None, ErrorDetail>>> CommandOneCondition { get; private set; }
    public Expression<Func<TModel, bool>> CommandFilter { get; private set; }
    public Func<IQueryable<TModel>, IQueryable<TModel>> CommandSpecialAction { get; private set; }
    public Func<TModel, Task> UpdateOneFunc { get; private set; }
    public ErrorDetail NullErrorDetail { get; private set; }
    public ErrorDetail SaveChangesErrorDetail { get; private set; }
    public Func<TModel, TResult> ResultFunc { get; private set; }

    public ICommandOneFlowBuilderResult<TModel, TResult> WithResultIfSucceed(Func<TModel, TResult> resultFunc)
    {
        ResultFunc = resultFunc;
        return this;
    }

    public ICreateOneConditionResult<TModel, TResult> CreateOne(Func<Task<TModel>> modelFunc)
    {
        CommandTypeOne = CommandTypeOne.Create;
        ModelCreateFunc = modelFunc;
        return this;
    }

    public ICreateOneConditionResult<TModel, TResult> CreateOne(Func<TModel> modelFunc)
    {
        CommandTypeOne = CommandTypeOne.Create;
        ModelCreateFunc = () => Task.FromResult(modelFunc.Invoke());
        return this;
    }

    public ICreateOneConditionResult<TModel, TResult> CreateOne(TModel model)
    {
        CommandTypeOne = CommandTypeOne.Create;
        ModelCreateFunc = () => Task.FromResult(model);
        return this;
    }

    public IUpdateOneSpecialActionResult<TModel, TResult> UpdateOne(Expression<Func<TModel, bool>> filter)
    {
        CommandTypeOne = CommandTypeOne.Update;
        CommandFilter = filter;
        return this;
    }

    public IRemoveOneSpecialActionResult<TModel, TResult> RemoveOne(Expression<Func<TModel, bool>> filter)
    {
        CommandTypeOne = CommandTypeOne.Remove;
        CommandFilter = filter;
        return this;
    }

    ISaveChangesOneErrorDetailResult<TModel, TResult> ICreateOneConditionResult<TModel, TResult>.WithCondition(
        Func<TModel, OneOf<None, ErrorDetail>> condition)
    {
        CommandOneCondition = model => Task.FromResult(condition.Invoke(model));
        return this;
    }

    ICommandOneErrorDetailResult<TModel, TResult> IRemoveOneConditionResult<TModel, TResult>.WithCondition(
        Func<TModel, Task<OneOf<None, ErrorDetail>>> conditionAsync)
    {
        CommandOneCondition = conditionAsync;
        return this;
    }

    ICommandOneErrorDetailResult<TModel, TResult> IRemoveOneConditionResult<TModel, TResult>.WithCondition(
        Func<TModel, OneOf<None, ErrorDetail>> condition)
    {
        CommandOneCondition = model => Task.FromResult(condition.Invoke(model));
        return this;
    }

    IUpdateOneModifyResult<TModel, TResult> IUpdateOneConditionResult<TModel, TResult>.WithCondition(
        Func<TModel, Task<OneOf<None, ErrorDetail>>> conditionAsync)
    {
        CommandOneCondition = conditionAsync;
        return this;
    }

    IUpdateOneModifyResult<TModel, TResult> IUpdateOneConditionResult<TModel, TResult>.WithCondition(
        Func<TModel, OneOf<None, ErrorDetail>> condition)
    {
        CommandOneCondition = model => Task.FromResult(condition.Invoke(model));
        return this;
    }

    ISaveChangesOneErrorDetailResult<TModel, TResult> ICreateOneConditionResult<TModel, TResult>.WithCondition(
        Func<TModel, Task<OneOf<None, ErrorDetail>>> conditionAsync)
    {
        CommandOneCondition = conditionAsync;
        return this;
    }

    IUpdateOneConditionResult<TModel, TResult> IUpdateOneSpecialActionResult<TModel, TResult>.WithSpecialAction(
        Func<IQueryable<TModel>, IQueryable<TModel>> specialAction)
    {
        CommandSpecialAction = specialAction;
        return this;
    }

    IRemoveOneConditionResult<TModel, TResult> IRemoveOneSpecialActionResult<TModel, TResult>.WithSpecialAction(
        Func<IQueryable<TModel>, IQueryable<TModel>> specialAction)
    {
        CommandSpecialAction = specialAction;
        return this;
    }

    public ICommandOneErrorDetailResult<TModel, TResult> WithModify(Func<TModel, Task> updateFuncAsync)
    {
        UpdateOneFunc = updateFuncAsync;
        return this;
    }

    public ICommandOneErrorDetailResult<TModel, TResult> WithModify(Action<TModel> updateFunc)
    {
        UpdateOneFunc = model =>
        {
            updateFunc.Invoke(model);
            return Task.CompletedTask;
        };
        return this;
    }

    public ISaveChangesOneErrorDetailResult<TModel, TResult> WithErrorIfNull(ErrorDetail errorDetail)
    {
        NullErrorDetail = errorDetail;
        return this;
    }

    public ISaveChangesOneSucceed<TModel, TResult> WithErrorIfSaveChange(ErrorDetail errorDetail)
    {
        SaveChangesErrorDetail = errorDetail;
        return this;
    }
}