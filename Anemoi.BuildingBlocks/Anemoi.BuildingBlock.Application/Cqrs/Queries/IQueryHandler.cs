using Anemoi.BuildingBlock.Application.EventDriven;
using MediatR;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries;

public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>, IMessageHandler
    where TQuery : IQuery<TResult>;