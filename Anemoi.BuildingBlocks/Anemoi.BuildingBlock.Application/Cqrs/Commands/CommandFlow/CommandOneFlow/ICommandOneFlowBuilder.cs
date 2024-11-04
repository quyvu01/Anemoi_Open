using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Results;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;

public interface ICommandOneFlowBuilderResult<TModel, out TResult> where TModel : class
{
    CommandTypeOne CommandTypeOne { get; }
    Func<Task<TModel>> ModelCreateFunc { get; }
    Func<TModel, Task<OneOf<None, ErrorDetail>>> CommandOneCondition { get; }
    Expression<Func<TModel, bool>> CommandFilter { get; }
    Func<IQueryable<TModel>, IQueryable<TModel>> CommandSpecialAction { get; }
    Func<TModel, Task> UpdateOneFunc { get; }
    ErrorDetail NullErrorDetail { get; }
    ErrorDetail SaveChangesErrorDetail { get; }
    Func<TModel, TResult> ResultFunc { get; }
}
public interface ICommandOneFlowBuilderVoid<TModel> where TModel : class
{
    CommandTypeOne CommandTypeOne { get; }
    Func<Task<TModel>> ModelCreateFunc { get; }
    Func<TModel, Task<OneOf<None, ErrorDetail>>> CommandOneCondition { get; }
    Expression<Func<TModel, bool>> CommandFilter { get; }
    Func<IQueryable<TModel>, IQueryable<TModel>> CommandSpecialAction { get; }
    Func<TModel, Task> UpdateOneFunc { get; }
    ErrorDetail NullErrorDetail { get; }
    ErrorDetail SaveChangesErrorDetail { get; }
}