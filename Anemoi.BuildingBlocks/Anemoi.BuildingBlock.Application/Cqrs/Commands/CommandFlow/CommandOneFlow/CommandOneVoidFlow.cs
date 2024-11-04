using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Results;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;

public class CommandOneVoidFlow<TModel> :
    IStartOneCommandVoid<TModel>,
    ICreateOneConditionVoid<TModel>,
    IUpdateOneSpecialActionVoid<TModel>,
    IRemoveOneSpecialActionVoid<TModel>,
    IUpdateOneConditionVoid<TModel>,
    IRemoveOneConditionVoid<TModel>,
    IUpdateOneModifyVoid<TModel>,
    ICommandOneErrorDetailVoid<TModel>,
    ISaveChangesOneErrorDetailVoid<TModel>,
    ICommandOneFlowBuilderVoid<TModel>
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

    public ICreateOneConditionVoid<TModel> CreateOne(Func<Task<TModel>> modelFunc)
    {
        CommandTypeOne = CommandTypeOne.Create;
        ModelCreateFunc = modelFunc;
        return this;
    }

    public ICreateOneConditionVoid<TModel> CreateOne(Func<TModel> modelFunc)
    {
        CommandTypeOne = CommandTypeOne.Create;
        ModelCreateFunc = () => Task.FromResult(modelFunc.Invoke());
        return this;
    }

    public ICreateOneConditionVoid<TModel> CreateOne(TModel model)
    {
        CommandTypeOne = CommandTypeOne.Create;
        ModelCreateFunc = () => Task.FromResult(model);
        return this;
    }

    public IUpdateOneSpecialActionVoid<TModel> UpdateOne(Expression<Func<TModel, bool>> filter)
    {
        CommandTypeOne = CommandTypeOne.Update;
        CommandFilter = filter;
        return this;
    }

    public IRemoveOneSpecialActionVoid<TModel> RemoveOne(Expression<Func<TModel, bool>> filter)
    {
        CommandTypeOne = CommandTypeOne.Remove;
        CommandFilter = filter;
        return this;
    }

    ISaveChangesOneErrorDetailVoid<TModel> ICreateOneConditionVoid<TModel>.WithCondition(
        Func<TModel, OneOf<None, ErrorDetail>> condition)
    {
        CommandOneCondition = model => Task.FromResult(condition.Invoke(model));
        return this;
    }

    ICommandOneErrorDetailVoid<TModel> IRemoveOneConditionVoid<TModel>.WithCondition(
        Func<TModel, Task<OneOf<None, ErrorDetail>>> conditionAsync)
    {
        CommandOneCondition = conditionAsync;
        return this;
    }

    ICommandOneErrorDetailVoid<TModel> IRemoveOneConditionVoid<TModel>.WithCondition(
        Func<TModel, OneOf<None, ErrorDetail>> condition)
    {
        CommandOneCondition = model => Task.FromResult(condition.Invoke(model));
        return this;
    }

    IUpdateOneModifyVoid<TModel> IUpdateOneConditionVoid<TModel>.WithCondition(
        Func<TModel, Task<OneOf<None, ErrorDetail>>> conditionAsync)
    {
        CommandOneCondition = conditionAsync;
        return this;
    }

    IUpdateOneModifyVoid<TModel> IUpdateOneConditionVoid<TModel>.WithCondition(
        Func<TModel, OneOf<None, ErrorDetail>> condition)
    {
        CommandOneCondition = model => Task.FromResult(condition.Invoke(model));
        return this;
    }

    ISaveChangesOneErrorDetailVoid<TModel> ICreateOneConditionVoid<TModel>.WithCondition(
        Func<TModel, Task<OneOf<None, ErrorDetail>>> conditionAsync)
    {
        CommandOneCondition = conditionAsync;
        return this;
    }

    IUpdateOneConditionVoid<TModel> IUpdateOneSpecialActionVoid<TModel>.WithSpecialAction(
        Func<IQueryable<TModel>, IQueryable<TModel>> specialAction)
    {
        CommandSpecialAction = specialAction;
        return this;
    }

    IRemoveOneConditionVoid<TModel> IRemoveOneSpecialActionVoid<TModel>.WithSpecialAction(
        Func<IQueryable<TModel>, IQueryable<TModel>> specialAction)
    {
        CommandSpecialAction = specialAction;
        return this;
    }

    public ICommandOneErrorDetailVoid<TModel> WithModify(Func<TModel, Task> updateFuncAsync)
    {
        UpdateOneFunc = updateFuncAsync;
        return this;
    }

    public ICommandOneErrorDetailVoid<TModel> WithModify(Action<TModel> updateFunc)
    {
        UpdateOneFunc = model =>
        {
            updateFunc.Invoke(model);
            return Task.CompletedTask;
        };
        return this;
    }

    public ISaveChangesOneErrorDetailVoid<TModel> WithErrorIfNull(ErrorDetail errorDetail)
    {
        NullErrorDetail = errorDetail;
        return this;
    }

    public ICommandOneFlowBuilderVoid<TModel> WithErrorIfSaveChange(ErrorDetail errorDetail)
    {
        SaveChangesErrorDetail = errorDetail;
        return this;
    }
}