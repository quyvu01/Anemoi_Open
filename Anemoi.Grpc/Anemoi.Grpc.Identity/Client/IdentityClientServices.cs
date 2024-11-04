using Grpc.Core;
using Grpc.Net.Client;

namespace Anemoi.Grpc.Identity.Client;

public static class IdentityClientServices
{
    public static async Task<AuthenticateResult> LoginAsync(string identityAddress, LoginRequest request,
        CancellationToken token = default)
    {
        using var channel = GrpcChannel.ForAddress(identityAddress);
        var client = new IdentityService.IdentityServiceClient(channel);
        var result = await client.LoginAsync(request, cancellationToken: token);
        return result;
    }

    public static async Task<CommandWithVoidResult> LogOutAsync(string identityAddress, string accessToken,
        CancellationToken token = default)
    {
        using var channel = GrpcChannel.ForAddress(identityAddress);
        var client = new IdentityService.IdentityServiceClient(channel);
        var metadata = new Metadata();
        if (!string.IsNullOrEmpty(accessToken)) metadata.Add("Authorization", $"Bearer {accessToken}");
        var result = await client.LogOutAsync(new LogOutRequest(), metadata, cancellationToken: token);
        return result;
    }
}