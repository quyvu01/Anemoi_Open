using System.Diagnostics.CodeAnalysis;
using Anemoi.BuildingBlock.Application.Errors;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries.QueryFlow.QueryOneFlow;

public interface IQueryOneErrorDetail<TModel, out TResponse> where TModel : class where TResponse : class
{
    IQueryOneFlowBuilder<TModel, TResponse> WithErrorIfNull([NotNull] ErrorDetail errorDetail);
}