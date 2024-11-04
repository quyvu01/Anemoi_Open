using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using OneOf;

namespace Anemoi.Centralize.Application.Abstractions;

public interface IRequestClientService
{
    Task<CountingResponse> GetCountingResponseAsync<TQuery>(TQuery query,
        CancellationToken token = default) where TQuery : class, IQueryCounting;

    Task<OneOf<TResponse, ErrorDetailResponse>> GetSingleResponseAsync<TQuery, TResponse>(TQuery query,
        CancellationToken token = default) where TResponse : class
        where TQuery : class, IQuery<OneOf<TResponse, ErrorDetailResponse>>;

    Task<PaginationResponse<TResponse>> GetPaginationResponseAsync<TQuery, TResponse>(TQuery query,
        CancellationToken token = default) where TResponse : class
        where TQuery : class, IQuery<PaginationResponse<TResponse>>;

    Task<CollectionResponse<TResponse>> GetCollectionResponseAsync<TQuery, TResponse>(TQuery query,
        CancellationToken token = default) where TResponse : class
        where TQuery : class, IQuery<CollectionResponse<TResponse>>;

    Task<OneOf<TResponse, ErrorDetailResponse>> SendCommandAsync<TCommand, TResponse>(TCommand command,
        CancellationToken token = default) where TCommand : class, ICommand<OneOf<TResponse, ErrorDetailResponse>>
        where TResponse : class;

    Task<OneOf<None, ErrorDetailResponse>> SendCommandAsync<TCommand>(TCommand command,
        CancellationToken token = default) where TCommand : class, ICommand<OneOf<None, ErrorDetailResponse>>;
}