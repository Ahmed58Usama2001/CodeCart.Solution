using CodeCart.Core.Services.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StackExchange.Redis;
using System.Text.Json;

namespace CodeCart.Service;

public class ResponseCacheService(IConnectionMultiplexer redis) : IResponseCacheService
{
    private readonly IDatabase _database = redis.GetDatabase(1);

    public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
    {
        var option = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var serializedResponse = JsonSerializer.Serialize(response, option);

        await _database.StringSetAsync(cacheKey, serializedResponse, timeToLive);
    }

    public async Task<string?> GetCachedResponseAsync<T>(string cacheKey)
    {
        var cachedResponse = await _database.StringGetAsync(cacheKey);

        if (cachedResponse.IsNullOrEmpty)
            return null;    

        return cachedResponse;
    }

    public async Task RemoveCacheByPattern(string pattern)
    {
        var server = redis.GetServer(redis.GetEndPoints().FirstOrDefault() ?? throw new InvalidOperationException("No Redis server found."));
        var keys = server.Keys(database: 1, pattern:$"*{pattern}*").ToArray();

        if (keys.Length != 0)
        {
            await _database.KeyDeleteAsync(keys);
        }
    }
}
