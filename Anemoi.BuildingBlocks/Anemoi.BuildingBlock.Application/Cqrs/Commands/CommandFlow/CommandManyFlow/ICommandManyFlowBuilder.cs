using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.BuildingBlock.Application.Results;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;

public interface ICommandManyFlowBuilderResult<TModel, out TResult> where TModel : class
{
    CommandTypeMany CommandTypeMany { get; }
    Func<Task<List<TModel>>> ModelsCreateFunc { get; }
    Func<List<TModel>, Task<OneOf<None, ErrorDetail>>> CommandManyCondition { get; }
    Expression<Func<TModel, bool>> CommandFilter { get; }
    Func<IQueryable<TModel>, IQueryable<TModel>> CommandSpecialAction { get; }
    Func<List<TModel>, Task> UpdateManyFunc { get; }
    ErrorDetail NullErrorDetail { get; }
    ErrorDetail SaveChangesErrorDetail { get; }
    Func<List<TModel>, TResult> ResultFunc { get; }
}

public interface ICommandManyFlowBuilderVoid<TModel> where TModel : class
{
    CommandTypeMany CommandTypeMany { get; }
    Func<Task<List<TModel>>> ModelsCreateFunc { get; }
    Func<List<TModel>, Task<OneOf<None, ErrorDetail>>> CommandManyCondition { get; }
    Expression<Func<TModel, bool>> CommandFilter { get; }
    Func<IQueryable<TModel>, IQueryable<TModel>> CommandSpecialAction { get; }
    Func<List<TModel>, Task> UpdateManyFunc { get; }
    ErrorDetail NullErrorDetail { get; }
    ErrorDetail SaveChangesErrorDetail { get; }
}