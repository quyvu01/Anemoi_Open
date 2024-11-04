using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using MediatR;
using OneOf;

namespace Anemoi.Centralize.Application.Abstractions;

public interface IQueryCountingHandler<in TQuery> :
    IQueryHandler<TQuery, CountingResponse>
    where TQuery : class, IQueryCounting
{
    public IRequestClientService RequestClientService { get; }

    Task<CountingResponse> IRequestHandler<TQuery, CountingResponse>
        .Handle(TQuery request, CancellationToken cancellationToken)
        => RequestClientService.GetCountingResponseAsync(request, cancellationToken);
}

public interface IQueryOneHandler<in TQuery, TResponse> :
    IQueryHandler<TQuery, OneOf<TResponse, ErrorDetailResponse>>
    where TResponse : class
    where TQuery : class, IQuery<OneOf<TResponse, ErrorDetailResponse>>
{
    public IRequestClientService RequestClientService { get; }

    Task<OneOf<TResponse, ErrorDetailResponse>> IRequestHandler<TQuery, OneOf<TResponse, ErrorDetailResponse>>
        .Handle(TQuery request, CancellationToken cancellationToken) =>
        RequestClientService.GetSingleResponseAsync<TQuery, TResponse>(request, cancellationToken);
}

public interface IQueryCollectionHandler<in TQuery, TResponse> :
    IQueryHandler<TQuery, CollectionResponse<TResponse>>
    where TResponse : class
    where TQuery : class, IQuery<CollectionResponse<TResponse>>
{
    public IRequestClientService RequestClientService { get; }

    Task<CollectionResponse<TResponse>> IRequestHandler<TQuery, CollectionResponse<TResponse>>
        .Handle(TQuery request, CancellationToken cancellationToken) =>
        RequestClientService.GetCollectionResponseAsync<TQuery, TResponse>(request, cancellationToken);
}

public interface IQueryPagedHandler<in TQuery, TResponse> :
    IQueryHandler<TQuery, PaginationResponse<TResponse>>
    where TResponse : class
    where TQuery : class, IQuery<PaginationResponse<TResponse>>
{
    public IRequestClientService RequestClientService { get; }

    Task<PaginationResponse<TResponse>> IRequestHandler<TQuery, PaginationResponse<TResponse>>
        .Handle(TQuery request, CancellationToken cancellationToken) =>
        RequestClientService.GetPaginationResponseAsync<TQuery, TResponse>(request, cancellationToken);
}

public interface ICommandVoidHandler<in TCommand> :
    ICommandHandler<TCommand, OneOf<None, ErrorDetailResponse>>
    where TCommand : class, ICommand<OneOf<None, ErrorDetailResponse>>
{
    public IRequestClientService RequestClientService { get; }

    Task<OneOf<None, ErrorDetailResponse>> IRequestHandler<TCommand, OneOf<None, ErrorDetailResponse>>
        .Handle(TCommand request, CancellationToken cancellationToken)
        => RequestClientService.SendCommandAsync(request, cancellationToken);
}

public interface ICommandResultHandler<in TCommand, TResponse> :
    ICommandHandler<TCommand, OneOf<TResponse, ErrorDetailResponse>>
    where TCommand : class, ICommand<OneOf<TResponse, ErrorDetailResponse>>
    where TResponse : class
{
    public IRequestClientService RequestClientService { get; }

    Task<OneOf<TResponse, ErrorDetailResponse>> IRequestHandler<TCommand, OneOf<TResponse, ErrorDetailResponse>>
        .Handle(TCommand request, CancellationToken cancellationToken)
        => RequestClientService.SendCommandAsync<TCommand, TResponse>(request, cancellationToken);
}