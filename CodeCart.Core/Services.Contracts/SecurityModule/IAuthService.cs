using CodeCart.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace CodeCart.Core.Services.Contracts.SecurityModule;

public interface IAuthService
{
    Task<string> CreateAccessTokenAsync(AppUser user, UserManager<AppUser> userManager);
}

