using CodeCart.Core.Entities.Identity;
using CodeCart.Core.Entities.Identity.Enums;
using CodeCart.Core.Entities.Identity.Gmail;
using CodeCart.Core.Services.Contract.AccountModuleContracts;
using CodeCart.Infrastructure.Data;
using CodeCart.Service.AuthModuleService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace CodeCart.Service.AuthModuleService;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly StoreContext _context;
    private readonly GoogleAuthConfig _googleAuthConfig;
    private readonly ILogger<GoogleAuthService> _logger;


    public GoogleAuthService(
        UserManager<AppUser> userManager,
        StoreContext context,
        IOptions<GoogleAuthConfig> googleAuthConfig,
                ILogger<GoogleAuthService> logger

        )
    {
        _userManager = userManager;
        _context = context;
        _googleAuthConfig = googleAuthConfig.Value;
        _logger = logger;

    }

    public async Task<AppUser> GoogleSignIn(GoogleSignInVM model)
    {

        Payload payload = new();

        try
        {
            payload = await ValidateAsync(model.IdToken, new ValidationSettings
            {
                Audience = new[] { model.ClientId }
            });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }

        var userToBeCreated = new CreateUserFromSocialLogin
        {
            UserName = payload.GivenName,
            Email = payload.Email,
            ProfilePicture = payload.Picture,
            LoginProviderSubject = payload.Subject,
        };

        var user = await _userManager.CreateUserFromSocialLogin(_context, userToBeCreated, LoginProvider.Google);

        if (user is not null)
            return user;

        else
            return null;
    }
}