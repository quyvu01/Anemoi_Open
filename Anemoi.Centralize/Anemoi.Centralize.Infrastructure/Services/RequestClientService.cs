using System.Text.Json;
using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.BuildingBlock.Application.Cqrs.Queries;
using Anemoi.BuildingBlock.Application.EventDriven;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.Centralize.Application.Abstractions;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OneOf;

namespace Anemoi.Centralize.Infrastructure.Services;

public sealed class RequestClientService(
    IServiceProvider serviceProvider,
    IHttpContextAccessor httpContextAccessor,
    ICustomUserIdGetter customUserIdGetter,
    ICustomWorkspaceIdGetter customWorkspaceIdGetter)
    : IRequestClientService
{
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(180);

    public async Task<CountingResponse> GetCountingResponseAsync<TQuery>(TQuery query,
        CancellationToken token = default)
        where TQuery : class, IQueryCounting
    {
        var client = serviceProvider.GetRequiredService<IRequestClient<TQuery>>();
        var result = await client.GetResponse<CountingResponse>(query, SetRequestHead, token, RequestTimeout);
        return result.Message;
    }

    public async Task<OneOf<TResponse, ErrorDetailResponse>> GetSingleResponseAsync<TQuery, TResponse>(TQuery query,
        CancellationToken token = default) where TResponse : class
        where TQuery : class, IQuery<OneOf<TResponse, ErrorDetailResponse>>
    {
        var client = serviceProvider.GetRequiredService<IRequestClient<TQuery>>();
        var result = await client
            .GetResponse<TResponse, ErrorDetailResponse>(query, SetRequestHead, token, RequestTimeout);
        return result.Message switch
        {
            TResponse response => response,
            _ => result.Message as ErrorDetailResponse
        };
    }

    public async Task<PaginationResponse<TResponse>> GetPaginationResponseAsync<TQuery, TResponse>(TQuery query,
        CancellationToken token = default) where TResponse : class
        where TQuery : class, IQuery<PaginationResponse<TResponse>>
    {
        var client = serviceProvider.GetRequiredService<IRequestClient<TQuery>>();
        var result = await client
            .GetResponse<PaginationResponse<TResponse>>(query, SetRequestHead, token, RequestTimeout);
        return result.Message;
    }

    public async Task<CollectionResponse<TResponse>> GetCollectionResponseAsync<TQuery, TResponse>(TQuery query,
        CancellationToken token = default) where TQuery : class, IQuery<CollectionResponse<TResponse>>
        where TResponse : class
    {
        var client = serviceProvider.GetRequiredService<IRequestClient<TQuery>>();
        var result = await client
            .GetResponse<CollectionResponse<TResponse>>(query, SetRequestHead, token, RequestTimeout);
        return result.Message;
    }

    public async Task<OneOf<TResponse, ErrorDetailResponse>> SendCommandAsync<TCommand, TResponse>(TCommand command,
        CancellationToken token = default) where TCommand : class, ICommand<OneOf<TResponse, ErrorDetailResponse>>
        where TResponse : class
    {
        var client = serviceProvider.GetRequiredService<IRequestClient<TCommand>>();
        var result = await client
            .GetResponse<TResponse, ErrorDetailResponse>(command, SetRequestHead, token, RequestTimeout);
        return result.Message switch
        {
            TResponse response => response,
            _ => result.Message as ErrorDetailResponse
        };
    }

    public async Task<OneOf<None, ErrorDetailResponse>> SendCommandAsync<TCommand>(TCommand command,
        CancellationToken token = default) where TCommand : class, ICommand<OneOf<None, ErrorDetailResponse>>
    {
        var client = serviceProvider.GetRequiredService<IRequestClient<TCommand>>();
        var result =
            await client.GetResponse<IVoid, ErrorDetailResponse>(command, SetRequestHead, token, RequestTimeout);
        return result.Message switch
        {
            IVoid => None.Value,
            _ => result.Message as ErrorDetailResponse
        };
    }

    private void SetRequestHead<TRequest>(IRequestPipeConfigurator<TRequest> x) where TRequest : class
    {
        SetUserId(x);
        SetWorkspaceId(x);
        SetAdministrator(x);
        SetApplicationPolicy(x);
        SetToken(x);
    }

    private void SetUserId<TRequest>(IRequestPipeConfigurator<TRequest> x) where TRequest : class =>
        x.UseExecute(ctx =>
            ctx.Headers.Set("userId", customUserIdGetter.UserId ?? httpContextAccessor.HttpContext.GetUserId()));

    private void SetWorkspaceId<TRequest>(IRequestPipeConfigurator<TRequest> x) where TRequest : class =>
        x.UseExecute(ctx => ctx.Headers.Set("workspaceId",
            customWorkspaceIdGetter.WorkspaceId ?? httpContextAccessor.HttpContext.GetWorkspaceId()));

    private void SetAdministrator<TRequest>(IRequestPipeConfigurator<TRequest> x) where TRequest : class =>
        x.UseExecute(ctx => ctx.Headers.Set("administrator", SetAdministrator()));

    private void SetApplicationPolicy<TRequest>(IRequestPipeConfigurator<TRequest> x) where TRequest : class =>
        x.UseExecute(ctx => ctx.Headers.Set("applicationPolicy", SetApplicationPolicy()));

    private void SetToken<TRequest>(IRequestPipeConfigurator<TRequest> x) where TRequest : class =>
        x.UseExecute(ctx => ctx.Headers.Set("token", httpContextAccessor.HttpContext.GetToken()));

    private string SetAdministrator()
    {
        var roles = httpContextAccessor.HttpContext.GetUserRoles();
        var isSystemRoles = roles?.Contains("Administrator") ?? false;
        return $"{isSystemRoles}";
    }

    private string SetApplicationPolicy()
    {
        var applicationPolicy = httpContextAccessor.HttpContext.GetApplicationPolicy();
        return JsonSerializer.Serialize(applicationPolicy);
    }
}