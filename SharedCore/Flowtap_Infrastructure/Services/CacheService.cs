using System.Text.Json;
using Flowtap_Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Flowtap_Infrastructure.Services;

public class CacheService(IDistributedCache cache, IConnectionMultiplexer redis) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        var bytes = await cache.GetAsync(key, ct);
        if (bytes is null || bytes.Length == 0) return null;

        return JsonSerializer.Deserialize<T>(bytes, JsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default) where T : class
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
        var options = new DistributedCacheEntryOptions();

        if (expiry.HasValue)
            options.AbsoluteExpirationRelativeToNow = expiry;

        await cache.SetAsync(key, bytes, options, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
        => await cache.RemoveAsync(key, ct);

    public async Task RemoveByPatternAsync(string pattern, CancellationToken ct = default)
    {
        var db = redis.GetDatabase();
        var server = redis.GetServer(redis.GetEndPoints().First());

        var keys = server.Keys(pattern: $"*{pattern}*").ToArray();
        if (keys.Length > 0)
            await db.KeyDeleteAsync(keys);
    }
}
