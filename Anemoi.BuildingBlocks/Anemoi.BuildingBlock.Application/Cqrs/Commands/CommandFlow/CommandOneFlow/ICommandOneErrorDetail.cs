using System.Diagnostics.CodeAnalysis;
using Anemoi.BuildingBlock.Application.Errors;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;

public interface ICommandOneErrorDetailResult<TModel, TResult> where TModel : class
{
    ISaveChangesOneErrorDetailResult<TModel, TResult> WithErrorIfNull([NotNull] ErrorDetail errorDetail);
}

public interface ICommandOneErrorDetailVoid<TModel> where TModel : class
{
    ISaveChangesOneErrorDetailVoid<TModel> WithErrorIfNull([NotNull] ErrorDetail errorDetail);
}