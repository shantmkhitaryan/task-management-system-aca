using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace task_management_system_aca.Services;

public class RedisCacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

   
    public async Task SetAsync(string key, object value, int seconds)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(seconds)
        };

        var jsonData = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, jsonData, options);
    }

   
    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(data))
            return default;

        return JsonSerializer.Deserialize<T>(data);
    }

    
    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}