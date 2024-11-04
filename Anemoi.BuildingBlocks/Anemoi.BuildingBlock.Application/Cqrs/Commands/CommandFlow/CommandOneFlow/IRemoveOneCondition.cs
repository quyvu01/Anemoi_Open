using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Results;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;

public interface IRemoveOneConditionResult<TModel, TResult> where TModel : class
{
    ICommandOneErrorDetailResult<TModel, TResult> WithCondition(
        System.Func<TModel, OneOf<None, ErrorDetail>> condition);

    ICommandOneErrorDetailResult<TModel, TResult> WithCondition(
        System.Func<TModel, Task<OneOf<None, ErrorDetail>>> conditionAsync);
}

public interface IRemoveOneConditionVoid<TModel> where TModel : class
{
    ICommandOneErrorDetailVoid<TModel> WithCondition(System.Func<TModel, OneOf<None, ErrorDetail>> condition);

    ICommandOneErrorDetailVoid<TModel> WithCondition(
        System.Func<TModel, Task<OneOf<None, ErrorDetail>>> conditionAsync);
}