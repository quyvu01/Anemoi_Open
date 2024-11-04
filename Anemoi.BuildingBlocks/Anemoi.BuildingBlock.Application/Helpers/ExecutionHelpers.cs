using System;
using System.Threading.Tasks;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Helpers;

public static class ExecutionHelpers
{
    public static async Task<TResponse> GetOrAddAsync<TResponse, TError>(
        Func<Task<OneOf<TResponse, TError>>> getFunc, Func<Task> setFunc, int retryTimes)
    {
        var retryPolicy = Extensions.Extensions.ExponentialRetryPolicy<Exception>(retryTimes, _ => TimeSpan.Zero,
            (_, _) => { });

        var result = await retryPolicy.ExecuteAsync(InnerFunc);
        return result;

        async Task<TResponse> InnerFunc()
        {
            var getResult = await getFunc.Invoke();
            if (getResult.IsT0) return getResult.AsT0;
            await setFunc.Invoke();
            throw new Exception();
        }
    }
}