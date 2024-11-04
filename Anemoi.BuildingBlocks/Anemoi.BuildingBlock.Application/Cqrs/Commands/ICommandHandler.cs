using Anemoi.BuildingBlock.Application.EventDriven;
using MediatR;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand> where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>, IMessageHandler
    where TCommand : ICommand<TResult>;