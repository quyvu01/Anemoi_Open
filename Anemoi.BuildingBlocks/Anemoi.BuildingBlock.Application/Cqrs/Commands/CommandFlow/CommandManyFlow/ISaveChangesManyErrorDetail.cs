using System.Diagnostics.CodeAnalysis;
using Anemoi.BuildingBlock.Application.Errors;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;

public interface ISaveChangesManyErrorDetailResult<TModel, TResult> where TModel : class
{
    ISaveChangesManySucceedResult<TModel, TResult> WithErrorIfSaveChange([NotNull] ErrorDetail errorDetail);
}

public interface ISaveChangesManyErrorDetailVoid<TModel> where TModel : class
{
    ICommandManyFlowBuilderVoid<TModel> WithErrorIfSaveChange([NotNull] ErrorDetail errorDetail);
}