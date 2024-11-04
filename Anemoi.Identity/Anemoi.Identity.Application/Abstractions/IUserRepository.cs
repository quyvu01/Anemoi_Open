using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.Identity.Domain.Models;
using OneOf;

namespace Anemoi.Identity.Application.Abstractions;

public interface IUserRepository
{
    Task<OneOf<None, Exception>> SetLockoutEnabledAsync(User user, bool enabled);
    Task<string> GenerateEmailConfirmationTokenAsync(User user);

    Task<bool> ValidatePasswordAsync(User user, string password,
        CancellationToken cancellationToken);

    Task<string> GenerateChangePhoneNumberTokenAsync(User user);
    Task<OneOf<None, Exception>> ConfirmEmailAsync(User user, string token);
    Task<OneOf<None, Exception>> ConfirmPhoneNumberAsync(User user, string token);

    Task<OneOf<None, Exception>> SetLockoutEndDateAsync(User user,
        DateTimeOffset? lockUntil);

    Task<OneOf<None, Exception>> ChangePasswordAsync(User user, string currentPassword,
        string newPassword);

    Task<string> GeneratePasswordResetTokenAsync(User user);
    Task<IList<string>> GetRolesAsync(User user);

    Task<OneOf<None, Exception>> AddToRolesAsync(User user,
        IEnumerable<string> rolesName);

    Task<OneOf<None, Exception>> RemoveFromRolesAsync(User user, IEnumerable<string> rolesName);

    Task<OneOf<None, Exception>> ResetPasswordAsync(User user, string tokenPassword,
        string newPassword);

    Task RemoveUser(User user);
    Task<string> GenerateEmailResetPassTokenAsync(User user);
    Task<OneOf<None, Exception>> ConfirmEmailResetPassAsync(User user, string commandToken);
}