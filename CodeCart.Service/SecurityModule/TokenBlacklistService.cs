using CodeCart.Core.Services.Contracts.SecurityModule;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;

namespace CodeCart.Service.SecurityModule;

public class TokenBlacklistService(IConnectionMultiplexer redis) : ITokenBlacklistService
{
    private readonly IDatabase _database = redis.GetDatabase();

    public async Task BlacklistTokenAsync(string token)
    {
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var jti = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

        if (string.IsNullOrEmpty(jti))
            return;

        var key = $"blacklisted_token:{jti}";
        var expiration = jwtToken.ValidTo - DateTime.UtcNow;

        if (expiration > TimeSpan.Zero)
        {
            await _database.StringSetAsync(key, "blacklisted", expiration);
        }
    }

    public async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var jti = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

        if (string.IsNullOrEmpty(jti))
            return false;

        var key = $"blacklisted_token:{jti}";
        return await _database.KeyExistsAsync(key);
    }
}