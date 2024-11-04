using System.Diagnostics.CodeAnalysis;
using Anemoi.BuildingBlock.Application.Errors;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;

public interface ISaveChangesOneErrorDetailResult<TModel, TResult> where TModel : class
{
    ISaveChangesOneSucceed<TModel, TResult> WithErrorIfSaveChange([NotNull] ErrorDetail errorDetail);
}

public interface ISaveChangesOneErrorDetailVoid<TModel> where TModel : class
{
    ICommandOneFlowBuilderVoid<TModel> WithErrorIfSaveChange([NotNull] ErrorDetail errorDetail);
}