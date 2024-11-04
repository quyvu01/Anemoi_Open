using Anemoi.BuildingBlock.Application.EventDriven;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using MassTransit;
using MediatR;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Cqrs.Commands;

[ExcludeFromTopology]
public interface ICommand : IRequest, IMessage;

[ExcludeFromTopology]
public interface ICommand<out TResult> : IRequest<TResult>, IMessage;

[ExcludeFromTopology]
public interface ICommandVoid : ICommand<OneOf<None, ErrorDetailResponse>>;

[ExcludeFromTopology]
public interface ICommandResult<TResult> : ICommand<OneOf<TResult, ErrorDetailResponse>> where TResult : class;