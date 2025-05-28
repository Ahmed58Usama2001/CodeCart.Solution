using CodeCart.Core.Entities.Identity;
using CodeCart.Core.Entities.Identity.Gmail;

namespace CodeCart.Core.Services.Contract.AccountModuleContracts;

public interface IGoogleAuthService
{
    Task<AppUser> GoogleSignIn(GoogleSignInVM model);
}
