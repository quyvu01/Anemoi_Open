using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.Repositories;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Domain.Models;
using Anemoi.Identity.Infrastructure.DataContext;
using Microsoft.AspNetCore.Identity;
using OneOf;
using Serilog;

namespace Anemoi.Identity.Infrastructure.Repositories;

public sealed class UserRepository(
    UserManager<User> userManager,
    IdentityDbContext dbContext,
    ILogger logger)
    : EfRepository<User>(dbContext, logger), IUserRepository
{
    public async Task<OneOf<None, Exception>> SetLockoutEnabledAsync(
        User user,
        bool enabled)
    {
        var setResult = await userManager.SetLockoutEnabledAsync(user, enabled);
        if (setResult.Succeeded) return None.Value;
        return new Exception(string.Join(",", setResult.Errors.Select(x => x.Description)));
    }

    public Task<string> GenerateEmailConfirmationTokenAsync(User user) =>
        userManager.GenerateUserTokenAsync(user, "email", "confirm");

    public async Task<bool> ValidatePasswordAsync(User user, string password,
        CancellationToken cancellationToken)
    {
        var validators = userManager.PasswordValidators;
        foreach (var validator in validators)
        {
            var isValidPassword = await validator.ValidateAsync(userManager, user, password);
            if (isValidPassword.Succeeded) continue;
            return false;
        }

        return true;
    }

    public Task<string> GenerateEmailResetPassTokenAsync(User user) =>
        userManager.GenerateUserTokenAsync(user, "email", "reset");

    public Task<string> GenerateChangePhoneNumberTokenAsync(User user) =>
        userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber!);

    public async Task<OneOf<None, Exception>> ConfirmEmailAsync(User user, string token)
    {
        var confirmResult = await userManager.VerifyUserTokenAsync(user, "email", "confirm", token);
        if (confirmResult) return None.Value;
        return new Exception("Confirm email failed!");
    }

    public async Task<OneOf<None, Exception>> ConfirmEmailResetPassAsync(User user, string token)
    {
        var confirmResult = await userManager.VerifyUserTokenAsync(user, "email", "reset", token);
        if (confirmResult) return None.Value;
        return new Exception("Confirm email failed!");
    }

    public async Task<OneOf<None, Exception>> ConfirmPhoneNumberAsync(User user, string token)
    {
        var confirmResult = await userManager
            .VerifyChangePhoneNumberTokenAsync(user, token, user.PhoneNumber!);
        if (confirmResult) return None.Value;
        return new Exception("Confirm phone number error!");
    }

    public async Task<OneOf<None, Exception>> ChangePasswordAsync(
        User user,
        string currentPassword, string newPassword)
    {
        var updateResult = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (updateResult.Succeeded) return None.Value;
        return new Exception(string.Join(",", updateResult.Errors.Select(x => x.Description)));
    }

    public Task<string> GeneratePasswordResetTokenAsync(User user) =>
        userManager.GeneratePasswordResetTokenAsync(user);

    public async Task<IList<string>> GetRolesAsync(User user)
    {
        var roles = await userManager.GetRolesAsync(user);
        return roles;
    }

    public async Task<OneOf<None, Exception>> AddToRolesAsync(
        User user,
        IEnumerable<string> rolesName)
    {
        var addResult = await userManager.AddToRolesAsync(user, rolesName);
        if (addResult.Succeeded) return None.Value;
        return new Exception(string.Join(",", addResult.Errors.Select(x => x.Description)));
    }

    public async Task<OneOf<None, Exception>> RemoveFromRolesAsync(
        User user,
        IEnumerable<string> rolesName)
    {
        var removeResult = await userManager.RemoveFromRolesAsync(user, rolesName);
        if (removeResult.Succeeded) return None.Value;
        return new Exception(string.Join(",", removeResult.Errors.Select(x => x.Description)));
    }

    public async Task<OneOf<None, Exception>> ResetPasswordAsync(
        User user,
        string tokenPassword, string newPassword)
    {
        var resetResult = await userManager.ResetPasswordAsync(user, tokenPassword, newPassword);
        if (resetResult.Succeeded) return None.Value;
        return new Exception(string.Join(",", resetResult.Errors.Select(x => x.Description)));
    }

    public async Task RemoveUser(User user)
    {
        await userManager.DeleteAsync(user);
    }

    public async Task<OneOf<None, Exception>> SetLockoutEndDateAsync(
        User user,
        DateTimeOffset? lockUntil)
    {
        var lockResult = await userManager.SetLockoutEndDateAsync(user, lockUntil);
        if (lockResult.Succeeded) return None.Value;
        return new Exception(string.Join(",", lockResult.Errors.Select(x => x.Description)));
    }
}