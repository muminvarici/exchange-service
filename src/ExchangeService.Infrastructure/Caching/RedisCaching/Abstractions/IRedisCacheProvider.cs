using ExchangeService.Core.Abstractions.Caching;

namespace ExchangeService.Infrastructure.Caching.RedisCaching.Abstractions;

public interface IRedisCacheProvider : ICacheProvider
{
    Task<bool> LockAsync(string key, CacheTime cacheTime, string value);
    Task<bool> UnlockAsync(string key, string value);
}