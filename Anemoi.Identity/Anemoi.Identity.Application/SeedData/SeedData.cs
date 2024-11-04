using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.Contract.Identity.Commands.UserCommands.CreateUser;
using Anemoi.Contract.Identity.ModelIds;
using MediatR;
using Microsoft.Extensions.Configuration;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Application.Configurations;
using Anemoi.Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable All

namespace Lambda.Identity.Application.SeedData;

public static class SeedData
{
    public static async Task RegisterAdministratorAsync(IServiceScope serviceScope)
    {
        const string administrator = "Administrator";
        var userRepository = serviceScope.ServiceProvider.GetRequiredService<IUserRepository>();
        var userDbRepository = serviceScope.ServiceProvider.GetRequiredService<ISqlRepository<User>>();
        var mediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();
        var userClaimRepository = serviceScope.ServiceProvider.GetRequiredService<IUserClaimRepository>();
        var config = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
        var seedUserData = config.GetSection(nameof(SeedUserData)).Get<SeedUserData>();
        var defaultApplicationPolicies = serviceScope.ServiceProvider.GetRequiredService<DefaultApplicationPolices>();
        var users = seedUserData.SupperAdminUsers;
        var internalUser = defaultApplicationPolicies.ApplicationPolicies.First();
        foreach (var user in users)
        {
            var existUser = await userDbRepository.GetFirstByConditionAsync(x => x.Email == user.UserName);
            if (existUser is { })
            {
                var userRoles = await userRepository.GetRolesAsync(existUser);
                if (!userRoles.Contains(administrator))
                    await userRepository.AddToRolesAsync(existUser, [administrator]);
                var claims = await userClaimRepository
                    .GetUserClaimsAsync(existUser.UserId, CancellationToken.None);
                if (claims.Any(x => x.Type == internalUser.Key && x.Value == internalUser.Value))
                    continue;
                await userClaimRepository.AddClaimsAsync(existUser.UserId,
                    [new Claim(internalUser.Key, internalUser.Value)], CancellationToken.None);
                continue;
            }

            var newAdminCommand = new CreateUserCommand
            {
                Password = user.Password, FirstName = user.FirstName,
                LastName = user.LastName, Email = user.UserName
            };
            var newUserResult = await mediator.Send(newAdminCommand);
            if (newUserResult.IsT1) continue;
            var createdUser = await userDbRepository.GetFirstByConditionAsync(x =>
                x.UserId == new UserId(Guid.Parse(newUserResult.AsT0.Id)));
            await userRepository.AddToRolesAsync(createdUser, [administrator]);
            await userClaimRepository.AddClaimsAsync(createdUser.UserId,
                [new Claim(internalUser.Key, internalUser.Value)], CancellationToken.None);
        }
    }

    public static async Task SeedRolesAsync(IServiceScope serviceScope)
    {
        var roleRepository = serviceScope.ServiceProvider.GetRequiredService<ISqlRepository<Role>>();
        var unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var defaultApplicationPolicies = serviceScope.ServiceProvider.GetRequiredService<DefaultApplicationPolices>();

        var roles = defaultApplicationPolicies.ApplicationPolicies
            .SelectMany(x => x.Roles).Distinct();
        var isChangeed = false;
        foreach (var role in roles)
        {
            if (await roleRepository.ExistByConditionAsync(x => role == x.Name)) continue;
            var roleUser = new Role { Name = role, RoleId = new RoleId(IdGenerator.NextGuid()) };
            isChangeed = true;
            await roleRepository.CreateOneAsync(roleUser);
        }

        if (isChangeed) await unitOfWork.SaveChangesAsync();
    }

    public static async Task SeedApplicationPoliciesAsync(IServiceScope serviceScope)
    {
        var identityPolicyRepository = serviceScope.ServiceProvider
            .GetRequiredService<ISqlRepository<IdentityPolicy>>();
        var unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var roleRepository = serviceScope.ServiceProvider.GetRequiredService<ISqlRepository<Role>>();

        var defaultApplicationPolicies = serviceScope.ServiceProvider.GetRequiredService<DefaultApplicationPolices>();
        var systemRoles = await roleRepository.GetQueryable().AsNoTracking().ToListAsync();

        var roles = defaultApplicationPolicies.ApplicationPolicies
            .SelectMany(x => x.Roles).Distinct();
        var isChangeed = false;
        foreach (var applicationPolicy in defaultApplicationPolicies.ApplicationPolicies)
        {
            var existOne = await identityPolicyRepository.ExistByConditionAsync(x =>
                x.Key == applicationPolicy.Key && x.Value == applicationPolicy.Value);
            if (existOne) continue;
            isChangeed = true;
            var identityPolicyMapRoles = applicationPolicy.Roles
                .Select(a => new IdentityPolicyMapRole
                {
                    Id = new IdentityPolicyMapRoleId(IdGenerator.NextGuid()),
                    UserRoleId = systemRoles.FirstOrDefault(x => x.Name == a)?.RoleId
                });
            var newOne = new IdentityPolicy
            {
                Id = new IdentityPolicyId(IdGenerator.NextGuid()), Key = applicationPolicy.Key,
                Value = applicationPolicy.Value, IdentityPolicyMapRoles = identityPolicyMapRoles.ToList()
            };
            await identityPolicyRepository.CreateOneAsync(newOne);
        }

        if (isChangeed) await unitOfWork.SaveChangesAsync();
    }
}