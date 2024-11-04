using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Results;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;

public class CommandManyWithVoidFlow<TModel> :
    IStartManyCommandVoid<TModel>,
    ICreateManyConditionVoid<TModel>,
    IUpdateManySpecialActionVoid<TModel>,
    IRemoveManySpecialActionVoid<TModel>,
    IUpdateManyConditionVoid<TModel>,
    IRemoveManyConditionVoid<TModel>,
    IUpdateManyModifyVoid<TModel>,
    ICommandManyErrorDetailVoid<TModel>,
    ISaveChangesManyErrorDetailVoid<TModel>,
    ICommandManyFlowBuilderVoid<TModel>
    where TModel : class
{
    public ICreateManyConditionVoid<TModel> CreateMany(Func<Task<List<TModel>>> modelsFunc)
    {
        CommandTypeMany = CommandTypeMany.Create;
        ModelsCreateFunc = modelsFunc;
        return this;
    }

    public ICreateManyConditionVoid<TModel> CreateMany(Func<List<TModel>> modelsFunc)
    {
        CommandTypeMany = CommandTypeMany.Create;
        ModelsCreateFunc = () => Task.FromResult(modelsFunc.Invoke());
        return this;
    }

    public ICreateManyConditionVoid<TModel> CreateMany(List<TModel> models)
    {
        CommandTypeMany = CommandTypeMany.Create;
        ModelsCreateFunc = () => Task.FromResult(models);
        return this;
    }

    public ISaveChangesManyErrorDetailVoid<TModel> WithCondition(
        Func<List<TModel>, OneOf<None, ErrorDetail>> condition)
    {
        CommandManyCondition = models => Task.FromResult(condition.Invoke(models));
        return this;
    }

    public ISaveChangesManyErrorDetailVoid<TModel> WithCondition(
        Func<List<TModel>, Task<OneOf<None, ErrorDetail>>> conditionAsync)
    {
        CommandManyCondition = conditionAsync;
        return this;
    }

    IUpdateManyModifyVoid<TModel> IUpdateManyConditionVoid<TModel>.WithCondition(
        Func<List<TModel>, Task<OneOf<None, ErrorDetail>>> conditionAsync)
    {
        CommandManyCondition = conditionAsync;
        return this;
    }

    ISaveChangesManyErrorDetailVoid<TModel> IRemoveManyConditionVoid<TModel>.WithCondition(
        Func<List<TModel>, OneOf<None, ErrorDetail>> condition)
    {
        CommandManyCondition = models => Task.FromResult(condition.Invoke(models));
        return this;
    }

    ISaveChangesManyErrorDetailVoid<TModel> IRemoveManyConditionVoid<TModel>.WithCondition(
        Func<List<TModel>, Task<OneOf<None, ErrorDetail>>> conditionAsync)
    {
        CommandManyCondition = conditionAsync;
        return this;
    }

    IUpdateManyModifyVoid<TModel> IUpdateManyConditionVoid<TModel>.WithCondition(
        Func<List<TModel>, OneOf<None, ErrorDetail>> condition)
    {
        CommandManyCondition = models => Task.FromResult(condition.Invoke(models));
        return this;
    }

    public IUpdateManySpecialActionVoid<TModel> UpdateMany(Expression<Func<TModel, bool>> filter)
    {
        CommandTypeMany = CommandTypeMany.Update;
        CommandFilter = filter;
        return this;
    }

    IUpdateManyConditionVoid<TModel> IUpdateManySpecialActionVoid<TModel>.WithSpecialAction(
        Func<IQueryable<TModel>, IQueryable<TModel>> specialAction)
    {
        CommandSpecialAction = specialAction;
        return this;
    }

    public ISaveChangesManyErrorDetailVoid<TModel> WithModify(Func<List<TModel>, Task> updateFuncAsync)
    {
        UpdateManyFunc = updateFuncAsync;
        return this;
    }

    public ISaveChangesManyErrorDetailVoid<TModel> WithModify(Action<List<TModel>> updateFunc)
    {
        UpdateManyFunc = models =>
        {
            updateFunc.Invoke(models);
            return Task.CompletedTask;
        };
        return this;
    }


    IRemoveManyConditionVoid<TModel> IRemoveManySpecialActionVoid<TModel>.WithSpecialAction(
        Func<IQueryable<TModel>, IQueryable<TModel>> specialAction)
    {
        CommandSpecialAction = specialAction;
        return this;
    }

    public IRemoveManySpecialActionVoid<TModel> RemoveMany(Expression<Func<TModel, bool>> filter)
    {
        CommandTypeMany = CommandTypeMany.Remove;
        CommandFilter = filter;
        return this;
    }

    ISaveChangesManyErrorDetailVoid<TModel> ICommandManyErrorDetailVoid<TModel>.WithErrorIfNull(
        ErrorDetail errorDetail)
    {
        NullErrorDetail = errorDetail;
        return this;
    }

    public ICommandManyFlowBuilderVoid<TModel> WithErrorIfSaveChange(ErrorDetail errorDetail)
    {
        SaveChangesErrorDetail = errorDetail;
        return this;
    }

    public ICommandManyFlowBuilderVoid<TModel> WithVoidIfSucceed(Func<List<TModel>> resultFunc)
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
    public Func<List<TModel>> ResultFunc { get; private set; }
}