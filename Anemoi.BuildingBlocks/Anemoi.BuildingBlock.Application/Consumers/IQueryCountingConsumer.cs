using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using MassTransit;
using MediatR;

namespace Anemoi.BuildingBlock.Application.Consumers;

public interface IQueryCountingConsumer<in TRequest> : IConsumer<TRequest>
    where TRequest : class, IQueryCounting
{
    public IMediator Mediator { get; }

    async Task IConsumer<TRequest>.Consume(ConsumeContext<TRequest> context)
    {
        var result = await Mediator.Send(context.Message, context.CancellationToken);
        await context.RespondAsync(result);
    }
}