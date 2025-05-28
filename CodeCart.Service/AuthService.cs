using CodeCart.Core.Entities;
using CodeCart.Core.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CodeCart.Service;

public class AuthService(IConfiguration configuration, IRefreshTokenService refreshTokenService) : IAuthService
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

        // Revoke the old refresh token
        await refreshTokenService.RevokeRefreshTokenAsync(refreshToken);

        // Generate new tokens
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

    // Legacy method for backward compatibility
    public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager)
    {
        return await CreateAccessTokenAsync(user, userManager);
    }
}