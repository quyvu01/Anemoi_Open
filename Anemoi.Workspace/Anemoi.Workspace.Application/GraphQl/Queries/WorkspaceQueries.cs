using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Responses;
using Anemoi.Contract.Identity.Queries.UserQueries.GetCrossCuttingUsers;
using Anemoi.Contract.Workspace.Responses;
using Anemoi.Workspace.Application.Attributes;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GreenDonut;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Anemoi.Workspace.Application.GraphQl.Queries;

public sealed class WorkspaceQueries
{
    [UseProjection]
    public IQueryable<WorkspaceResponse> GetWorkspaces(
        [Service] ISqlRepository<Domain.Models.Workspace> workspaceSqlRepository,
        [Service] IMapper mapper)
    {
        var projection = workspaceSqlRepository.GetQueryable()
            .ProjectTo<WorkspaceResponse>(mapper.ConfigurationProvider);
        return projection;
    }
}

public class WorkspaceResponseType : ObjectType<WorkspaceResponse>
{
    protected override void Configure(IObjectTypeDescriptor<WorkspaceResponse> descriptor)
    {
        descriptor.Field(t => t.UserName)
            .ResolveWith<Resolvers>(r => r.GetUserNameAsync(default!, default!, default));
        descriptor.Field(t => t.UserEmail)
            .ResolveWith<Resolvers>(r => r.GetUserEmailAsync(default!, default!, default));
    }
}

public class Resolvers
{
    [Resolver("userName")]
    public Task<string> GetUserNameAsync(
        [Parent] WorkspaceResponse workspace,
        UserNamesByIdDataLoader dataLoader,
        CancellationToken cancellationToken) => dataLoader.LoadAsync(workspace.UserId, cancellationToken);

    [Resolver("userEmail")]
    public Task<string> GetUserEmailAsync([Parent] WorkspaceResponse workspace,
        UserEmailsByIdDataLoader dataLoader,
        CancellationToken cancellationToken) => dataLoader.LoadAsync(workspace.UserId, cancellationToken);
}

public class UserNamesByIdDataLoader(
    IBatchScheduler batchScheduler,
    IServiceProvider serviceProvider,
    DataLoaderOptions options = null)
    : BatchDataLoader<string, string>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<string, string>> LoadBatchAsync(
        IReadOnlyList<string> keys, CancellationToken cancellationToken)
    {
        var userRequestClient = serviceProvider.GetRequiredService<IRequestClient<GetCrossCuttingUsersQuery>>();
        var userIds = keys.Where(a => Guid.TryParse(a, out _))
            .Select(Guid.Parse).ToList();
        var usersCollection = await userRequestClient
            .GetResponse<CollectionResponse<CrossCuttingDataResponse>>(
                new GetCrossCuttingUsersQuery(userIds, null), cancellationToken);

        return usersCollection.Message.Items.Where(p => keys.Contains(p.Id)).ToDictionary(p => p.Id, v => v.Value);
    }
}

public class UserEmailsByIdDataLoader(
    IBatchScheduler batchScheduler,
    IServiceProvider serviceProvider,
    DataLoaderOptions options = null)
    : BatchDataLoader<string, string>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<string, string>> LoadBatchAsync(
        IReadOnlyList<string> keys, CancellationToken cancellationToken)
    {
        var userRequestClient = serviceProvider.GetRequiredService<IRequestClient<GetCrossCuttingUsersQuery>>();
        var userIds = keys.Where(a => Guid.TryParse(a, out _))
            .Select(Guid.Parse).ToList();
        var usersCollection = await userRequestClient
            .GetResponse<CollectionResponse<CrossCuttingDataResponse>>(
                new GetCrossCuttingUsersQuery(userIds, "Email"), cancellationToken);

        return usersCollection.Message.Items.Where(p => keys.Contains(p.Id)).ToDictionary(p => p.Id, v => v.Value);
    }
}