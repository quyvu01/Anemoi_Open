using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandManyFlow;

public interface IUpdateManyModifyResult<TModel, TResult> where TModel : class
{
    ISaveChangesManyErrorDetailResult<TModel, TResult> WithModify([NotNull] Func<List<TModel>, Task> updateFuncAsync);
    ISaveChangesManyErrorDetailResult<TModel, TResult> WithModify([NotNull] Action<List<TModel>> updateFunc);
}

public interface IUpdateManyModifyVoid<TModel> where TModel : class
{
    ISaveChangesManyErrorDetailVoid<TModel> WithModify([NotNull] Func<List<TModel>, Task> updateFuncAsync);
    ISaveChangesManyErrorDetailVoid<TModel> WithModify([NotNull] Action<List<TModel>> updateFunc);
}