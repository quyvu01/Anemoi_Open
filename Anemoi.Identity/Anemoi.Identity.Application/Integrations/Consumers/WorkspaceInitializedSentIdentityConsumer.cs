using System;
using System.Linq;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Identity.Domain.Models;
using Anemoi.Orchestration.Contract.WorkspaceContract.Events.WorkspaceInitializedEvents;
using MassTransit;
using Serilog;

namespace Anemoi.Identity.Application.Integrations.Consumers;

public sealed class WorkspaceInitializedSentIdentityConsumer(
    ILogger logger,
    ISqlRepository<RoleGroup> roleGroupRepository,
    ISqlRepository<UserMapRoleGroup> userMapRoleGroupRepository,
    ISqlRepository<Role> userRoleRepository,
    IUnitOfWork unitOfWork) : IConsumer<WorkspaceInitializedSent>
{
    public async Task Consume(ConsumeContext<WorkspaceInitializedSent> context)
    {
        const string administratorRole = "Administrator";
        const string roleGroupClaimKey = "workspaceId";
        var message = context.Message;
        logger.Information("[WorkspaceInitializedSent] message: {@Message}", message);
        var defaultRoleGroup = await roleGroupRepository.GetFirstByConditionAsync(x =>
            x.RoleGroupClaims.Any(r => r.Value == message.CorrelationId.ToString()) && x.IsDefault);
        var userId = new UserId(Guid.Parse(message.UserId));
        if (defaultRoleGroup is { })
        {
            var existUserMapRoleGroup = await userMapRoleGroupRepository
                .GetFirstByConditionAsync(x => x.RoleGroupId == defaultRoleGroup.Id && x.UserId == userId);
            if (existUserMapRoleGroup is null)
            {
                await userMapRoleGroupRepository.CreateOneAsync(new UserMapRoleGroup
                {
                    Id = new UserMapRoleGroupId(IdGenerator.NextGuid()), UserId = userId,
                    RoleGroupId = defaultRoleGroup.Id
                });
                await unitOfWork.SaveChangesAsync();
            }

            await context
                .Publish<WorkspaceInitializedSentResult>(new
                    { message.CorrelationId, IsSucceed = true, RoleGroupId = defaultRoleGroup.Id.ToString() });
            return;
        }

        var role = await userRoleRepository
            .GetFirstByConditionAsync(x => x.Name == administratorRole);
        var roleGroupId = new RoleGroupId(IdGenerator.NextGuid());
        var roleGroupMapRole = new RoleGroupMapRole
        {
            Id = new RoleGroupMapUserRoleId(IdGenerator.NextGuid()), RoleGroupId = roleGroupId,
            RoleId = role?.RoleId
        };
        var userMapRoleGroup = new UserMapRoleGroup
        {
            Id = new UserMapRoleGroupId(IdGenerator.NextGuid()), RoleGroupId = roleGroupId, UserId = userId
        };
        var newRoleGroup = new RoleGroup
        {
            Id = roleGroupId, Name = "Administrator", IsDefault = true,
            SearchHint = "administrator", CreatedTime = DateTime.UtcNow, CreatorId = userId,
            RoleGroupMapRoles = [roleGroupMapRole],
            UserMapRoleGroups = [userMapRoleGroup],
            RoleGroupClaims =
            [
                new RoleGroupClaim
                {
                    Id = new RoleGroupClaimId(IdGenerator.NextGuid()), Key = roleGroupClaimKey,
                    Value = message.CorrelationId.ToString()
                }
            ]
        };
        await roleGroupRepository.CreateOneAsync(newRoleGroup);
        await unitOfWork.SaveChangesAsync();
        await context
            .Publish<WorkspaceInitializedSentResult>(new
                { message.CorrelationId, IsSucceed = true, RoleGroupId = roleGroupId.ToString() });
    }
}