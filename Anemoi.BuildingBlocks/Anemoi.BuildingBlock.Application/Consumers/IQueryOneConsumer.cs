using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using MassTransit;
using MediatR;

namespace Anemoi.BuildingBlock.Application.Consumers;

public interface IQueryOneConsumer<in TRequest, TResponse> : IConsumer<TRequest>
    where TRequest : class, IQueryOne<TResponse> where TResponse : class
{
    public IMediator Mediator { get; }

    async Task IConsumer<TRequest>.Consume(ConsumeContext<TRequest> context)
    {
        var result = await Mediator.Send(context.Message, context.CancellationToken);
        await result.Match(context.RespondAsync, context.RespondAsync);
    }
}