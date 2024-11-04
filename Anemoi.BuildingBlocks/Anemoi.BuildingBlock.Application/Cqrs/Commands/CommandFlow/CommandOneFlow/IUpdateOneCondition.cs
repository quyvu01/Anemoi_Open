using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Results;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;

public interface IUpdateOneConditionResult<TModel, TResult> where TModel : class
{
    IUpdateOneModifyResult<TModel, TResult> WithCondition(System.Func<TModel, OneOf<None, ErrorDetail>> condition);

    IUpdateOneModifyResult<TModel, TResult> WithCondition(
        System.Func<TModel, Task<OneOf<None, ErrorDetail>>> conditionAsync);
}

public interface IUpdateOneConditionVoid<TModel> where TModel : class
{
    IUpdateOneModifyVoid<TModel> WithCondition(System.Func<TModel, OneOf<None, ErrorDetail>> condition);
    IUpdateOneModifyVoid<TModel> WithCondition(System.Func<TModel, Task<OneOf<None, ErrorDetail>>> conditionAsync);
}