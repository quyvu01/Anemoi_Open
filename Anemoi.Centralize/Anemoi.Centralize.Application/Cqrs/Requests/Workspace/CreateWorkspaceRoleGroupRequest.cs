using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.Contract.Identity.ModelIds;

namespace Anemoi.Centralize.Application.Cqrs.Requests.Workspace;

public sealed record CreateWorkspaceRoleGroupRequest(
    string Name,
    string Description,
    List<RoleId> IdentityRoleIds) : ICommandVoid;