using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.Responses;

namespace Anemoi.Centralize.Application.Cqrs.Requests.Workspace;

public sealed record GenerateWorkspaceTokenRequest(string WorkspaceId) : ICommandResult<AuthenticationSuccessResponse>;