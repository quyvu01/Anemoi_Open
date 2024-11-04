using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.Repositories;
using Anemoi.Identity.Domain.Models;
using Anemoi.Identity.Infrastructure.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using OneOf;

namespace Anemoi.Identity.Infrastructure.Repositories;

public sealed class UserRoleRepository(
    RoleManager<Role> roleManager,
    IdentityDbContext dbContext,
    ILogger logger)
    : EfRepository<Role>(dbContext, logger)
{
    private readonly ILogger _logger = logger;

    public override async Task<OneOf<Role, Exception>> CreateOneAsync(Role item,
        CancellationToken token = default)
    {
        try
        {
            var createResult = await roleManager.CreateAsync(item);
            if (createResult.Succeeded) return item;
            return new Exception(string.Join(",", createResult.Errors.Select(x => x.Description)));
        }
        catch (Exception e)
        {
            _logger.Error("Error while creating a new role: {Error}", e.Message);
            return e;
        }
    }

    public override async Task<OneOf<None, Exception>> CreateManyAsync(List<Role> items,
        CancellationToken token = default)
    {
        foreach (var userRole in items)
        {
            var addRoleResult = await CreateOneAsync(userRole, token);
            if (addRoleResult.IsT1) return addRoleResult.AsT1;
        }

        return None.Value;
    }

    public override async Task<OneOf<None, Exception>> RemoveOneAsync(
        OneOf<Role, Expression<Func<Role, bool>>> itemOrFilter,
        CancellationToken token = default)
    {
        var role = await itemOrFilter.Match(Task.FromResult,
            f => GetFirstByConditionAsync(f, db => db.AsNoTracking(), token: token));
        try
        {
            var removeResult = await roleManager.DeleteAsync(role);
            if (!removeResult.Succeeded)
                return new Exception(string.Join(",", removeResult.Errors.Select(x => x.Description)));
            return None.Value;
        }
        catch (Exception e)
        {
            _logger.Error("Error while removing one userRole: {Error}", e.Message);
            return e;
        }
    }

    public override async Task<OneOf<None, Exception>> RemoveManyAsync(
        OneOf<List<Role>, Expression<Func<Role, bool>>> itemsOrFilter,
        CancellationToken token = default)
    {
        var roles = await itemsOrFilter.Match(Task.FromResult,
            f => GetManyByConditionAsync(f, db => db.AsNoTracking(), token: token));
        foreach (var userRole in roles)
        {
            var removeResult = await RemoveOneAsync(userRole, token);
            if (removeResult.IsT1) return removeResult.AsT1;
        }

        return None.Value;
    }
}