using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Queries;
using Anemoi.BuildingBlock.Application.Responses;
using MediatR;

namespace Anemoi.BuildingBlock.Application.Pipelines;

public sealed class PaginationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var result = await next.Invoke();
        if (request is not GetManyQuery query || result is not PaginationResponseGeneral response) return result;
        var take = query.GetTake();
        var totalRecord = response.TotalRecord;
        var totalPage = take is null or <= 0 ? 1 : (int)(totalRecord + take.Value - 1) / take.Value;
        response.TotalPage = totalPage;
        response.CurrentPageIndex = query.PageIndex ?? 1;
        return result;
    }
}