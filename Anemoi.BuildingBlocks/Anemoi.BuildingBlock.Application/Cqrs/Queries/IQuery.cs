using Anemoi.BuildingBlock.Application.EventDriven;
using Anemoi.BuildingBlock.Application.Responses;
using MassTransit;
using MediatR;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Cqrs.Queries;

[ExcludeFromTopology]
public interface IQuery<out TResult> : IRequest<TResult>, IMessage;

[ExcludeFromTopology]
public interface IQueryCounting : IQuery<CountingResponse>;

[ExcludeFromTopology]
public interface IQueryOne<TResult> : IQuery<OneOf<TResult, ErrorDetailResponse>>;

[ExcludeFromTopology]
public interface IQueryPaged<TResult> : IQuery<PaginationResponse<TResult>> where TResult : class;

[ExcludeFromTopology]
public interface IQueryCollection<TResult> : IQuery<CollectionResponse<TResult>> where TResult : class;