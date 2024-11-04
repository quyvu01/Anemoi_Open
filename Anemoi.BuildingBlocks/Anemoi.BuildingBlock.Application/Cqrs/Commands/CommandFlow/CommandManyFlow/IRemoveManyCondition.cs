using System.Collections.Generic;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Results;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;

public interface IRemoveManyConditionResult<TModel, TResult> where TModel : class
{
    ISaveChangesManyErrorDetailResult<TModel, TResult> WithCondition(
        System.Func<List<TModel>, OneOf<None, ErrorDetail>> condition);

    ISaveChangesManyErrorDetailResult<TModel, TResult> WithCondition(
        System.Func<List<TModel>, Task<OneOf<None, ErrorDetail>>> conditionAsync);
}

public interface IRemoveManyConditionVoid<TModel> where TModel : class
{
    ISaveChangesManyErrorDetailVoid<TModel> WithCondition(
        System.Func<List<TModel>, OneOf<None, ErrorDetail>> condition);

    ISaveChangesManyErrorDetailVoid<TModel> WithCondition(
        System.Func<List<TModel>, Task<OneOf<None, ErrorDetail>>> conditionAsync);
}