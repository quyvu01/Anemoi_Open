using System.Threading.Tasks;
using Anemoi.Identity.Application.ApplicationModels.Enums;
using Anemoi.Identity.Domain.Models;

namespace Anemoi.Identity.Application.Abstractions;

public interface ISignInRepository
{
    Task<SignInResult> PasswordSignInAsync(User user, string password,
        bool isPersistent, bool lockoutOnFailure);
    Task SignOutAsync();
    
}