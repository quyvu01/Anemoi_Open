using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.BuildingBlock.Application.EventDriven;
using MassTransit;
using MediatR;

namespace Anemoi.BuildingBlock.Application.Consumers;

public interface ICommandVoidConsumer<in TRequest> : IConsumer<TRequest> where TRequest : class, ICommandVoid
{
    public IMediator Mediator { get; }

    async Task IConsumer<TRequest>.Consume(ConsumeContext<TRequest> context)
    {
        var result = await Mediator.Send(context.Message, context.CancellationToken);
        await result.Match(_ => context.RespondAsync<IVoid>(new { }), context.RespondAsync);
    }
}