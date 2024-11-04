using System;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;

public interface ISaveChangesOneSucceed<TModel, TResult> where TModel : class
{
    ICommandOneFlowBuilderResult<TModel, TResult> WithResultIfSucceed(Func<TModel, TResult> resultFunc);
}