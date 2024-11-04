using System.Diagnostics.CodeAnalysis;
using Anemoi.BuildingBlock.Application.Errors;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;

public interface ICommandManyErrorDetailResult<TModel, TResult> where TModel : class
{
    ISaveChangesManyErrorDetailResult<TModel, TResult> WithErrorIfNull([NotNull] ErrorDetail errorDetail);
}
public interface ICommandManyErrorDetailVoid<TModel> where TModel : class
{
    ISaveChangesManyErrorDetailVoid<TModel> WithErrorIfNull([NotNull] ErrorDetail errorDetail);
}