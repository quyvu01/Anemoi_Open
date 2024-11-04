using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;

namespace Anemoi.BuildingBlock.Application.Pipelines;

public sealed class TimeMeasuringBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var stopWatch = Stopwatch.StartNew();
        var result = await next.Invoke();
        var timeSpent = stopWatch.ElapsedMilliseconds;
        Log.Information("Execute request: {@RequestName} spent: {@TimeSpent} ms!", typeof(TRequest).Name, timeSpent);
        return result;
    }
}