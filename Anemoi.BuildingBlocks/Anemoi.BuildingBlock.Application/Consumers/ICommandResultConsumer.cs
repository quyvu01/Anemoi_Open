using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using MassTransit;
using MediatR;

namespace Anemoi.BuildingBlock.Application.Consumers;

public interface ICommandResultConsumer<in TRequest, TResponse> : IConsumer<TRequest>
    where TRequest : class, ICommandResult<TResponse> where TResponse : class
{
    public IMediator Mediator { get; }

    async Task IConsumer<TRequest>.Consume(ConsumeContext<TRequest> context)
    {
        var result = await Mediator.Send(context.Message, context.CancellationToken);
        await result.Match(context.RespondAsync, context.RespondAsync);
    }
}