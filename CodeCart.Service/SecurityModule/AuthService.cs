using CodeCart.Core.Entities.Identity;
using CodeCart.Core.Entities.Identity.Enums;
using CodeCart.Core.Entities.Identity.Gmail;
using CodeCart.Core.Services.Contract.AccountModuleContracts;
using CodeCart.Core.Services.Contracts.SecurityModule;
using CodeCart.Infrastructure.Data;
using CodeCart.Service.AuthModuleService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CodeCart.Service.SecurityModule;

public class AuthService(IConfiguration configuration, IRefreshTokenService refreshTokenService,
    UserManager<AppUser> userManager,
    StoreContext context,
    IGoogleAuthService googleAuthService,
    IFacebookAuthService facebookAuthService) : IAuthService
{
    public async Task<TokenResult> CreateTokensAsync(AppUser user, UserManager<AppUser> userManager)
    {
        var accessToken = await CreateAccessTokenAsync(user, userManager);
        var refreshToken = await refreshTokenService.GenerateRefreshTokenAsync(user.Id);

        return new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<string> CreateAccessTokenAsync(AppUser user, UserManager<AppUser> userManager)
    {
        var authClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.GivenName, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userRoles = await userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
            authClaims.Add(new Claim(ClaimTypes.Role, role));

        var secretKey = Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]);
        var requiredKeyLength = 256 / 8;
        if (secretKey.Length < requiredKeyLength)
        {
            Array.Resize(ref secretKey, requiredKeyLength);
        }

        var token = new JwtSecurityToken(
            audience: configuration["JWT:ValidAudience"],
            issuer: configuration["JWT:ValidIssuer"],
            expires: DateTime.UtcNow.AddMinutes(double.Parse(configuration["JWT:AccessTokenDurationInMinutes"])),
            claims: authClaims,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken, UserManager<AppUser> userManager)
    {
        var tokenValidationResult = await refreshTokenService.ValidateRefreshTokenAsync(refreshToken);

        if (!tokenValidationResult.IsValid)
        {
            return new RefreshTokenResult
            {
                IsSuccess = false,
                ErrorMessage = "Invalid refresh token"
            };
        }

        var user = await userManager.FindByIdAsync(tokenValidationResult.UserId);
        if (user == null)
        {
            return new RefreshTokenResult
            {
                IsSuccess = false,
                ErrorMessage = "User not found"
            };
        }

        await refreshTokenService.RevokeRefreshTokenAsync(refreshToken);

        var tokens = await CreateTokensAsync(user, userManager);

        return new RefreshTokenResult
        {
            IsSuccess = true,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            User = user
        };
    }

    public async Task RevokeUserRefreshTokensAsync(string userId)
    {
        await refreshTokenService.RevokeAllUserRefreshTokensAsync(userId);
    }

    public async Task<AppUser> SignInWithGoogle(GoogleSignInVM model)
    {
        var response = await googleAuthService.GoogleSignIn(model);

        if (response is null)
            return new AppUser();

        return response;
    }

    public async Task<AppUser> SignInWithFacebook(FacebookSignInVM model)
    {
        var validatedFbToken = await facebookAuthService.ValidateFacebookToken(model.AccessToken);

        if (validatedFbToken is null)
            return new AppUser();

        var userInfo = await facebookAuthService.GetFacebookUserInformation(model.AccessToken);

        if (userInfo is null)
            return new AppUser();

        var userToBeCreated = new CreateUserFromSocialLogin
        {
            UserName = userInfo.Name,
            Email = userInfo.Email,
            ProfilePicture = userInfo.Picture.Data.Url.AbsoluteUri,
            LoginProviderSubject = userInfo.Id,
        };

        var user = await userManager.CreateUserFromSocialLogin(context, userToBeCreated, LoginProvider.Facebook);

        if (user is null)
            return new AppUser();

        return user;


    }


}