using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Domain.Models;
using SignInResult = Anemoi.Identity.Application.ApplicationModels.Enums.SignInResult;

namespace Anemoi.Identity.Infrastructure.Repositories;

public sealed class SignInRepository(SignInManager<User> signInManager) : ISignInRepository
{
    public async Task<SignInResult> PasswordSignInAsync(User user, string password,
        bool isPersistent, bool lockoutOnFailure)
    {
        var signInResult = await signInManager
            .PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        return signInResult switch
        {
            { IsLockedOut: true } => SignInResult.IsLockedOut,
            { IsNotAllowed: true } => SignInResult.IsNotAllowed,
            { RequiresTwoFactor: true } => SignInResult.RequiresTwoFactor,
            { Succeeded: true } => SignInResult.Succeeded,
            _ => throw new UnreachableException(nameof(signInResult))
        };
    }

    public Task SignOutAsync() => signInManager.SignOutAsync();
}