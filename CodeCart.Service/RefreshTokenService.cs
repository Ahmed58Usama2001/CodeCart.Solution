using CodeCart.Core.Services.Contracts;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Security.Cryptography;

namespace CodeCart.Service;

public class RefreshTokenService(IConnectionMultiplexer redis, IConfiguration configuration) : IRefreshTokenService
{
    private readonly IDatabase _database = redis.GetDatabase();
    private readonly int _refreshTokenExpirationDays = int.Parse(configuration["JWT:RefreshTokenDurationInDays"]);

    public async Task<string> GenerateRefreshTokenAsync(string userId)
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var refreshToken = Convert.ToBase64String(randomBytes);

        var key = $"refresh_token:{refreshToken}";
        var expiration = TimeSpan.FromDays(_refreshTokenExpirationDays);

        await _database.StringSetAsync(key, userId, expiration);

        // Also store in user's active tokens list for easy revocation
        var userTokensKey = $"user_refresh_tokens:{userId}";
        await _database.SetAddAsync(userTokensKey, refreshToken);
        await _database.KeyExpireAsync(userTokensKey, expiration);

        return refreshToken;
    }

    public async Task<RefreshTokenValidationResult> ValidateRefreshTokenAsync(string refreshToken)
    {
        var key = $"refresh_token:{refreshToken}";
        var userId = await _database.StringGetAsync(key);

        if (!userId.HasValue)
        {
            return new RefreshTokenValidationResult
            {
                IsValid = false
            };
        }

        return new RefreshTokenValidationResult
        {
            IsValid = true,
            UserId = userId
        };
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var key = $"refresh_token:{refreshToken}";
        var userId = await _database.StringGetAsync(key);

        if (userId.HasValue)
        {
            await _database.KeyDeleteAsync(key);

            // Remove from user's active tokens list
            var userTokensKey = $"user_refresh_tokens:{userId}";
            await _database.SetRemoveAsync(userTokensKey, refreshToken);
        }
    }

    public async Task RevokeAllUserRefreshTokensAsync(string userId)
    {
        var userTokensKey = $"user_refresh_tokens:{userId}";
        var refreshTokens = await _database.SetMembersAsync(userTokensKey);

        foreach (var token in refreshTokens)
        {
            var tokenKey = $"refresh_token:{token}";
            await _database.KeyDeleteAsync(tokenKey);
        }

        await _database.KeyDeleteAsync(userTokensKey);
    }
}