using CodeCart.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace CodeCart.Core.Services.Contracts.SecurityModule;

public interface IAuthService
{
    Task<string> CreateAccessTokenAsync(AppUser user, UserManager<AppUser> userManager);
    Task<TokenResult> CreateTokensAsync(AppUser user, UserManager<AppUser> userManager);
    Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken, UserManager<AppUser> userManager);
    Task RevokeUserRefreshTokensAsync(string userId);
}

public class TokenResult
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

public class RefreshTokenResult
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public AppUser User { get; set; }
}